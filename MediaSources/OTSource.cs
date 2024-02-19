using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using opentuner.MediaPlayers;
using opentuner.Utilities;

namespace opentuner.MediaSources
{
    public abstract class OTSource
    {
        public delegate void VideoChangeCallback(int video_number, bool start);
        public delegate void SourceDataChange(List<OTProperty> Properties);

        public abstract event SourceDataChange OnSourceData;

        // Request the Source Name (eg. Minitiouner)
        public abstract string GetName();

        // Request Device Name (eg. FTDI, Picotuner, etc)
        public abstract string GetDeviceName();

        // Request Source Description (can also include some info regarding its current settings)
        public abstract string GetDescription();

        // Shows a Source specific setting screen. Called when user clicks "Settings" in source selection screen.
        public abstract void ShowSettings();

        public abstract void SetFrequency(int device, uint frequency, uint symbol_rate, bool offset_included);
        public abstract long GetFrequency(int device, bool offset_included);

        public abstract void StartStreaming(int device);
        public abstract void StopStreaming(int device);
        public abstract int GetVideoSourceCount();
        public abstract CircularBuffer GetVideoDataQueue(int device);
        public abstract void RegisterTSConsumer(int device, CircularBuffer ts_buffer_queue);

        public abstract void Close();

        // initialize returns how many video players it need
        public abstract int Initialize(VideoChangeCallback VideoChangeCB, Control Parent);

        public abstract void ConfigureVideoPlayers(List<OTMediaPlayer> MediaPlayers);

        public abstract void ConfigureTSRecorders(List<TSRecorder> TSRecorders);

        public abstract void ConfigureTSStreamers(List<TSUdpStreamer> TSStreamers);

        public abstract void ConfigureMediaPath(string MediaPath);
        public abstract bool DeviceConnected { get; }

        //public abstract byte SelectHardwareInterface(int hardware_interface);


    }
}
