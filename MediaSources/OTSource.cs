using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opentuner
{
    public abstract class OTSource
    {
        public delegate void VideoChangeCallback(int video_number, bool start);

        public OTSource() { }
        public abstract long GetCurrentFrequency(int device, bool offset_included);
        public abstract void StartStreaming(int device);
        public abstract void StopStreaming(int device);
        public abstract int GetVideoSourceCount();
        public abstract string GetHardwareDescription();
        public abstract CircularBuffer GetVideoDataQueue(int device);
        public abstract void RegisterTSConsumer(int device, CircularBuffer ts_buffer_queue);

        public abstract void Close();
        public abstract bool Initialize(VideoChangeCallback VideoChangeCB, SourceStatusCallback SourceStatusCB);

        public abstract bool HardwareConnected { get; }

        // kinda temporay - needs a rework
        public abstract void change_frequency(byte tuner, UInt32 freq, UInt32 sr, bool lnb_supply, bool polarization_supply_horizontal, uint rf_input, bool tone_22kHz_P1);

        // temporary

        public abstract bool current_enable_lnb_supply { get; set; }
        public abstract bool current_enable_horiz_supply { get; set; }
        public abstract bool current_tone_22kHz_P1 { get; set; }

        public abstract uint current_rf_input_1 { get; set; }
        public abstract uint current_rf_input_2 { get; set; }

        public abstract uint current_frequency_1 { get; set; }
        public abstract uint current_sr_1 { get; set; }

        public abstract uint current_frequency_2 { get; set; }
        public abstract uint current_sr_2 { get; set; }

        public abstract int current_offset_A { get; set; }
        public abstract int current_offset_B { get; set; }

        public abstract bool Initialize(VideoChangeCallback VideoChangeCB, SourceStatusCallback SourceStatusCB, bool manual, string i2c_serial, string ts_serial, string ts2_serial);

        public abstract byte set_polarization_supply(byte lnb_num, bool supply_enable, bool supply_horizontal);

    }
}
