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

        public VideoViewForm(Control video_control, String title, int device_id, OTSource video_source, Control[] extra_controls)
        {
            InitializeComponent();

            _video_source = video_source;
            _title = title;
            _device_id = device_id;

            this.Text = title;


            foreach (Control control in extra_controls)
            {
                Controls.Add(control);
            }

            Controls.Add(video_control);
        }

    }
}
