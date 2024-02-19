using opentuner.MediaPlayers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace opentuner.MediaSources.Winterhill
{
    public class WinterhillSource : OTSource
    {
        WinterhillSettings _settings;

        public override event SourceDataChange OnSourceData;

        public override bool DeviceConnected => throw new NotImplementedException();

        public override void Close()
        {
            throw new NotImplementedException();
        }

        public override void ConfigureMediaPath(string MediaPath)
        {
            throw new NotImplementedException();
        }

        public override void ConfigureTSRecorders(List<TSRecorder> TSRecorders)
        {
            throw new NotImplementedException();
        }

        public override void ConfigureTSStreamers(List<TSUdpStreamer> TSStreamers)
        {
            throw new NotImplementedException();
        }

        public override void ConfigureVideoPlayers(List<OTMediaPlayer> MediaPlayers)
        {
            throw new NotImplementedException();
        }

        public override string GetDescription()
        {
            throw new NotImplementedException();
        }

        public override string GetDeviceName()
        {
            throw new NotImplementedException();
        }

        public override long GetFrequency(int device, bool offset_included)
        {
            throw new NotImplementedException();
        }

        public override string GetName()
        {
            return "Winterhill";
        }

        public override CircularBuffer GetVideoDataQueue(int device)
        {
            throw new NotImplementedException();
        }

        public override int GetVideoSourceCount()
        {
            return 4;
        }

        public override int Initialize(VideoChangeCallback VideoChangeCB, Control Parent)
        {
            throw new NotImplementedException();
        }

        public override void RegisterTSConsumer(int device, CircularBuffer ts_buffer_queue)
        {
            throw new NotImplementedException();
        }

        public override void SetFrequency(int device, uint frequency, uint symbol_rate, bool offset_included)
        {
            throw new NotImplementedException();
        }

        public override void ShowSettings()
        {
            WinterhillSettingsForm settingsForm = new WinterhillSettingsForm(_settings);
        }

        public override void StartStreaming(int device)
        {
            throw new NotImplementedException();
        }

        public override void StopStreaming(int device)
        {
            throw new NotImplementedException();
        }
    }
}
