using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LibVLCSharp.Shared;

namespace opentuner
{
    public class VLCMediaPlayer : OTMediaPlayer
    {


        LibVLC libVLC = new LibVLC("--aout=directsound");
        Media media;
        TSStreamMediaInput mediaInput;
        LibVLCSharp.WinForms.VideoView videoView;

        int player_volume;

        public override event EventHandler<MediaStatus> onVideoOut;

        CircularBuffer ts_data_queue;
        public VLCMediaPlayer(LibVLCSharp.WinForms.VideoView VideoView)
        {
            Console.WriteLine("VLC Media Player OT Called");

            videoView = VideoView;

        }

        private void MediaPlayer_EncounteredError(object sender, EventArgs e)
        {
            Console.WriteLine("VLC: Error: " + libVLC.LastLibVLCError);
        }

        private void MediaPlayer_Playing(object sender, EventArgs e)
        {
            Console.WriteLine("VLC: Playing ");
        }

        private void MediaPlayer_Stopped(object sender, EventArgs e)
        {
            Console.WriteLine("VLC: Stopped");
        }


        public override void Initialize(CircularBuffer TSDataQueue)
        {
            ts_data_queue = TSDataQueue;
            videoView.MediaPlayer = new MediaPlayer(libVLC);
            videoView.MediaPlayer.Stopped += MediaPlayer_Stopped;
            videoView.MediaPlayer.Playing += MediaPlayer_Playing;
            videoView.MediaPlayer.EncounteredError += MediaPlayer_EncounteredError;
            videoView.MediaPlayer.Vout += MediaPlayer_Vout;

            videoView.MediaPlayer.EnableMouseInput = false;
            videoView.MediaPlayer.EnableKeyInput = false;

            videoView.MediaPlayer.SetMarqueeInt(VideoMarqueeOption.Size, 20);
            videoView.MediaPlayer.SetMarqueeInt(VideoMarqueeOption.X, 10);
            videoView.MediaPlayer.SetMarqueeInt(VideoMarqueeOption.Y, 10);

            //videoView.MouseWheel += VideoView1_MouseWheel;

            mediaInput = new TSStreamMediaInput(TSDataQueue);
            media = new Media(libVLC, mediaInput);

            MediaConfiguration mediaConfig1 = new MediaConfiguration();
            mediaConfig1.EnableHardwareDecoding = false;
            media.AddOption(mediaConfig1);
        }

        private void MediaPlayer_Vout(object sender, MediaPlayerVoutEventArgs e)
        {
            // volume changes only take affect when media is playing
            videoView.MediaPlayer.Volume = player_volume;

            MediaStatus media_status = new MediaStatus();

            foreach (var track in media.Tracks)
            {
                switch (track.TrackType)
                {
                    case TrackType.Audio:
                        media_status.AudioChannels = track.Data.Audio.Channels;
                        media_status.AudioCodec = media.CodecDescription(TrackType.Audio, track.Codec);
                        media_status.AudioRate = track.Data.Audio.Rate;
                        break;
                    case TrackType.Video:
                        media_status.VideoCodec = media.CodecDescription(TrackType.Video, track.Codec);
                        media_status.VideoWidth = track.Data.Video.Width;
                        media_status.VideoHeight = track.Data.Video.Height;
                        break;
                }
            }

            if (onVideoOut != null)
            {
                onVideoOut(this, media_status);
            }

        }

        public override void SnapShot(string FileName)
        {
            videoView.MediaPlayer.TakeSnapshot(0, FileName, 0, 0);
        }


        public override void Close()
        {
            if (mediaInput != null)
                mediaInput.Dispose();
            if (media != null)
                media.Dispose();
        }

        public override void Stop()
        {
            if (videoView.MediaPlayer != null)
            {
                mediaInput.end = true;
                videoView.MediaPlayer.Stop();
            }

        }

        public override void Play()
        {
            if (videoView.MediaPlayer != null)
            {
                videoView.MediaPlayer.Stop();
            }

            ts_data_queue.Clear();

            mediaInput.ts_sync = false;
            mediaInput.end = false;

            if (videoView.MediaPlayer != null)
            {
                videoView.MediaPlayer.Play(media);
            }

        }
        public override void SetVolume(int Volume)
        {
            player_volume = Volume;

            if (videoView.MediaPlayer != null)
            {
                videoView.MediaPlayer.Volume = player_volume;
            }

        }

        public override int GetVolume()
        {
            return player_volume;
        }

    }
}
