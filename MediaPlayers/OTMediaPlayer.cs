using System;
using opentuner.Classes;
using opentuner.Transport;

namespace opentuner.MediaPlayers
{
    public abstract class OTMediaPlayer
    {
        public abstract event EventHandler<MediaStatus> onVideoOut;
        public abstract void Initialize(CircularBuffer TSDataQueue);
        public abstract void Close();
        public abstract void Play();
        public abstract void Stop();
        public abstract void SnapShot(string FileName);
        public abstract void SetVolume(int Volume);
        public abstract int GetVolume();
    }
}
