using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlyleafLib;
using FlyleafLib.Controls.WinForms;
using FlyleafLib.MediaPlayer;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using FlyleafLib;

namespace opentuner
{
   
    public class FFMPEGMediaPlayer : OTMediaPlayer
    {
        private int player_volume = 0;

        public override event EventHandler<MediaStatus> onVideoOut;

        public Player player { get; set; }
        public Config config { get; set; }

        FlyleafHost media_player;

        MediaStream media_stream;

        int counter = 0;
        CircularBuffer ts_data_queue;


        public FFMPEGMediaPlayer( FlyleafHost MediaPlayer )
        {
            media_player = MediaPlayer;

            config = new Config();
            config.Video.BackgroundColor = System.Windows.Media.Colors.Black;
            config.Demuxer.AllowTimeouts = false;

            config.Player.MinBufferDuration = TimeSpan.FromSeconds(1.5).Ticks;
            config.Decoder.MaxAudioFrames = 40;
            //config.Decoder.VideoThreads = 2;
            config.Demuxer.BufferDuration = TimeSpan.FromSeconds(10).Ticks;
            //config.Player.ThreadPriority = ThreadPriority.Highest;
            //config.Demuxer.AllowFindStreamInfo = false;

            /*
            config.Player.MinBufferDuration = TimeSpan.FromSeconds(1.5).Ticks;
            config.Demuxer.BufferDuration = TimeSpan.FromSeconds(10).Ticks;
            //config.Demuxer.AllowFindStreamInfo = false;
            config.Demuxer.FormatOpt["probesize"] = (5 * (long)1024 * 1024).ToString();
            config.Demuxer.FormatOpt["analyzeduration"] = (2 * (long)1000 * 1000).ToString();
            config.Decoder.MaxAudioFrames = 7;
            config.Decoder.MaxVideoFrames = 3;
            */

            player = new Player(config);

            media_player.Player = player;
            player.OpenCompleted += Player_OpenCompleted;
            player.PlaybackStopped += Player_PlaybackStopped;
            //player.BufferingStarted += Player_BufferingStarted;
            //player.PropertyChanged += Player_PropertyChanged;

            media_player.Enabled = true;
        }

        private void Player_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //Console.WriteLine("FFMPEG : Player Property Changed: " + e.PropertyName);
        }

        private void Player_BufferingStarted(object sender, EventArgs e)
        {
            //Console.WriteLine("FFMPEG : Buffering Started");
        }

        private void Player_PlaybackStopped(object sender, PlaybackStoppedArgs e)
        {
            Console.WriteLine("FFMPEG : Playback Stopped");
        }

        private void Player_OpenCompleted(object sender, OpenCompletedArgs e)
        {
            Console.WriteLine("FFMPEG : Open Completed");

            player.Audio.Volume = player_volume;

            MediaStatus media_status = new MediaStatus();

            media_status.VideoCodec = player.Video.Codec;
            media_status.VideoWidth = Convert.ToUInt32(player.Video.Width);
            media_status.VideoHeight = Convert.ToUInt32(player.Video.Height);
            media_status.AudioCodec = player.Audio.Codec;
            media_status.AudioChannels = Convert.ToUInt32(player.Audio.Channels);
            media_status.AudioRate = Convert.ToUInt32(player.Audio.SampleRate);

            if (onVideoOut != null)
            {
                onVideoOut(this, media_status);
            }
        }

        public override void Initialize(CircularBuffer TSDataQueue)
        {
            ts_data_queue = TSDataQueue;
            media_stream = new MediaStream(TSDataQueue);
            Console.WriteLine("MediaStream: Open");
        }

        public override void Close()
        {
        }

        public override void Play()
        {
            Console.WriteLine("FFMPEG: Playing");

            ts_data_queue.Clear();

            media_stream.ts_sync = false;
            media_stream.end = false;
            Console.WriteLine("FFMPEG Play");
            player.OpenAsync(media_stream);
            player.Play();
        }
        public override void Stop()
        {
            Console.WriteLine("FFMPEG Stop");
            media_stream.end = true;
            if (player.IsPlaying) { player.Stop(); }
        }

        public override void SnapShot(string FileName)
        {
            player.TakeSnapshotToFile(FileName);
        }

        public override void SetVolume(int Volume)
        {
            player_volume = Volume;
            player.Audio.Volume = Volume;
        }

        public override int GetVolume()
        {
            return player_volume;
        }

        public override string GetName()
        {
            return "FFMPEG";
        }
    }

    public class MediaStream : Stream
    {
        CircularBuffer ts_data_queue;
        public bool ts_sync = false;
        public bool end = false;

        BinaryWriter testFile = null;


        public MediaStream(CircularBuffer TSDataQueue) 
        {
            ts_data_queue = TSDataQueue;
        }

        public override bool CanRead => throw new NotImplementedException();

        public override bool CanSeek { get { return false; } }

        public override bool CanWrite { get { return false; } }

        public override long Length { get { return 0; } }

        public override long Position { set { }  get { return 0; } }

        public override void Flush()
        {
            Console.WriteLine("MediaStream: Flush");
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            //Console.WriteLine("Buffer: Len: " + buffer.Length.ToString() + "," + offset.ToString() + "," + count.ToString());

            while (ts_data_queue.Count< 10000)
            {
                if (end == true)
                {
                    return 0;
                }
                //Console.Write(".");
            }

            int queue_count = ts_data_queue.Count;   

            if (queue_count > 0)
            {
                byte raw_ts_data = 0;

                int buildLen = count-1;

                if (queue_count < buildLen)
                {
                    buildLen = queue_count;
                }

                int counter = 0;

                while (counter < buildLen)
                {
                    if (ts_data_queue.Count > 0)
                    {
                        raw_ts_data = ts_data_queue.Dequeue();

                        if (ts_sync == false && raw_ts_data != 0x47)
                        {
                            buildLen--;
                            continue;
                        }
                        else
                        {
                            ts_sync = true;
                            buffer[counter++] = raw_ts_data;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Warning: Trying to dequeue, but nothing available : ffmpeg: read : " + queue_count.ToString());
                    }
                }

                return buildLen;
            }

            Console.WriteLine("TS StreamInput: Shouldn't be here");
            return 0;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            
            Console.WriteLine("MediaStream: Seeking " + offset.ToString() + "," + origin.ToString());

            return 0;
        }

        public override void SetLength(long value)
        {
            Console.WriteLine("MediaStream: SetLength");
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            Console.WriteLine("MediaStream: Write");
        }
    }
}
