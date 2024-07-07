using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LibVLCSharp.Shared;
using Serilog;

namespace opentuner.MediaPlayers.VLC
{
    public class VLCMediaPlayer : OTMediaPlayer
    {
        LibVLC libVLC = new LibVLC("--aout=directsound");
        Media media;
        TSStreamMediaInput mediaInput;
        LibVLCSharp.WinForms.VideoView videoView;

        MediaPlayer _mediaplayer;

        int player_volume;

        public override event EventHandler<MediaStatus> onVideoOut;

        int _id = 0;

        CircularBuffer ts_data_queue;
        public VLCMediaPlayer(LibVLCSharp.WinForms.VideoView VideoView)
        {
            libVLC.Log += LibVLC_Log;
            videoView = VideoView;
        }

        private void LibVLC_Log(object sender, LogEventArgs e)
        {
            //Log.Information("VLCMediaPlayer: " + e.FormattedLog);
        }

        public override string GetName()
        {
            return "VLC";
        }

        // update mediaplayer reference invoking if required
        private delegate void updateMediaPlayerDelegate(MediaPlayer newPlayer, bool play);
        private void updateVideoPlayer(MediaPlayer newPlayer, bool play)
        {
            if (videoView.InvokeRequired)
            {
                updateMediaPlayerDelegate ump = new updateMediaPlayerDelegate(updateVideoPlayer);

                videoView.Invoke(ump, new object[] { newPlayer, play });
            }
            else
            {
                videoView.MediaPlayer = newPlayer;

                if (play)
                {
                    Thread.Sleep(10);
                    Log.Information("HWND: " + videoView.MediaPlayer.Hwnd.ToString());
                    videoView.MediaPlayer.Play(media);
                }
            }
        }




        private void MediaPlayer_EncounteredError(object sender, EventArgs e)
        {
            Log.Information("VLC: Error: " + libVLC.LastLibVLCError);
        }

        private void MediaPlayer_Playing(object sender, EventArgs e)
        {
            Log.Information("VLC: Playing ");
        }

        private void MediaPlayer_Stopped(object sender, EventArgs e)
        {
            Log.Information("VLC: Stopped");
        }
        

        public override void Initialize(CircularBuffer TSDataQueue)
        {
            ts_data_queue = TSDataQueue;

            _mediaplayer = new MediaPlayer(libVLC);
            _mediaplayer.Stopped += MediaPlayer_Stopped;
            _mediaplayer.Playing += MediaPlayer_Playing;
            _mediaplayer.EncounteredError += MediaPlayer_EncounteredError;
            _mediaplayer.Vout += MediaPlayer_Vout;

            _mediaplayer.EnableMouseInput = false;
            _mediaplayer.EnableKeyInput = false;

            _mediaplayer.SetMarqueeInt(VideoMarqueeOption.Size, 20);
            _mediaplayer.SetMarqueeInt(VideoMarqueeOption.X, 10);
            _mediaplayer.SetMarqueeInt(VideoMarqueeOption.Y, 10);

            mediaInput = new TSStreamMediaInput(ts_data_queue);
            media = new Media(libVLC, mediaInput);

            MediaConfiguration mediaConfig1 = new MediaConfiguration();
            mediaConfig1.EnableHardwareDecoding = false;
            media.AddOption(mediaConfig1);

            /*
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
            */
        }


        private void altPlay()
        {
            Log.Information("Alt Play Start");
            altStop();

            _mediaplayer = new MediaPlayer(libVLC);
            _mediaplayer.Stopped += MediaPlayer_Stopped;
            _mediaplayer.Playing += MediaPlayer_Playing;
            _mediaplayer.EncounteredError += MediaPlayer_EncounteredError;
            _mediaplayer.Vout += MediaPlayer_Vout;

            _mediaplayer.EnableMouseInput = false;
            _mediaplayer.EnableKeyInput = false;

            _mediaplayer.SetMarqueeInt(VideoMarqueeOption.Size, 20);
            _mediaplayer.SetMarqueeInt(VideoMarqueeOption.X, 10);
            _mediaplayer.SetMarqueeInt(VideoMarqueeOption.Y, 10);

            mediaInput = new TSStreamMediaInput(ts_data_queue);
            media = new Media(libVLC, mediaInput);

            MediaConfiguration mediaConfig1 = new MediaConfiguration();
            mediaConfig1.EnableHardwareDecoding = false;
            media.AddOption(mediaConfig1);

            updateVideoPlayer(_mediaplayer, true);
            Log.Information("Alt Play Stop");

        }

        private void altStop()
        {
            Log.Information("Alt Stop");

            if (_mediaplayer != null)
                _mediaplayer.Dispose();
            _mediaplayer = null;
            GC.Collect();
            GC.Collect();

            updateVideoPlayer(null, false);

            Log.Information("Alt Stop Done");

        }

        private void MediaPlayer_Vout(object sender, MediaPlayerVoutEventArgs e)
        {
            // volume changes only take affect when media is playing
            if (videoView.MediaPlayer == null)
            {
                return;
            }

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
                mediaInput.end = true;
                altStop();
        }

        public override void Play()
        {
            ts_data_queue.Clear();

            mediaInput.ts_sync = false;
            mediaInput.end = false;

            altPlay();
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

        public override int getID()
        {
            return _id;
        }

        public override void Initialize(CircularBuffer TSDataQueue, int ID)
        {
            _id = ID;
            Initialize(TSDataQueue);
        }
    }
}
