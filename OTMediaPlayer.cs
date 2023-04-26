using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opentuner
{
    public abstract class OTMediaPlayer
    {
        public abstract event EventHandler<MediaStatus> onVideoOut;
        public abstract void Initialize(ConcurrentQueue<byte> TSDataQueue);
        public abstract void Close();
        public abstract void Play();
        public abstract void Stop();
        public abstract void SnapShot(string FileName);
        public abstract void SetVolume(int Volume);
        public abstract int GetVolume();
    }
}
