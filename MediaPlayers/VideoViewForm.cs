using LibVLCSharp.Shared;
using opentuner.MediaSources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace opentuner
{
    public partial class VideoViewForm : Form
    {
        private OTSource _video_source;
        private int _device_id;
        private string _title;

        public VideoViewForm(Control VideoControl, String Title, int DeviceID, OTSource VideoSource)
        {
            InitializeComponent();
            _video_source = VideoSource;
            this.Text = Title;
            _title = Title;
            _device_id = DeviceID;
            this.Controls.Add(VideoControl);
            VideoSource.OnSourceData += VideoSource_OnSourceData;
        }

        private void VideoSource_OnSourceData(Dictionary<string, string> Properties, string topic)
        {
            
        }
    }
}
