using LibVLCSharp.Shared;
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

        private int tuner = 0;
        private int video_player = 0;

        public VideoViewForm(int videoPlayer, int _tuner)
        {
            InitializeComponent();

            switch(videoPlayer)
            {
                case 0: vlcPlayer.Visible = true; break;
                case 1: ffmpegPlayer.Visible = true; break;
            }
            
            video_player = videoPlayer;
            tuner = _tuner;

            this.Text = "Tuner " + tuner.ToString() + " - " + (video_player == 0 ? "VLC" : "FFMPEG");
        }

        public void updateStatus(TunerStatus status)
        {
            string info = "Tuner " + tuner.ToString() + " - " + (video_player == 0 ? "VLC" : "FFMPEG") + " : ";

            if (tuner == 1) { info += (status.T1P2_demod_status >= 2 ? "Locked" : "Not Locked "); }
            if (tuner == 2) { info += (status.T2P1_demod_status >= 2 ? "Locked" : "Not Locked "); }

            this.Text = info;
        }
    }
}
