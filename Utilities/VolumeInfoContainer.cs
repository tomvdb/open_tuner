using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Serilog;

namespace opentuner.Utilities
{
    public class VolumeInfoContainer : PictureBox
    {
        private int _last_volume = 0;
        private Timer _timer;

        public VolumeInfoContainer()
        {
            BackColor = Color.Black;
            Dock = DockStyle.Right;
            Width = 25;
            Visible = false;

            _timer = new Timer();
            _timer.Tick += _timer_Tick;
            _timer.Interval = 1500;
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            _timer.Stop();
            Visible = false;
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            // Do not call base.OnPaintBackground(pevent) to prevent clearing the background
        }

        public void UpdateVolume(int volume)
        {
            _last_volume = volume;
            Invalidate();

            Visible = true;
            _timer.Stop();
            _timer.Start();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);

            pe.Graphics.FillRectangle(Brushes.Black, 0, 0, Width, Height);
            pe.Graphics.DrawRectangle(Pens.White, 1, 1, Width-2, Height-2);

            int bar_size = (int)((Height - 7) * (_last_volume / 200.0));

            Rectangle barRect = new Rectangle(4, Height - 3 - bar_size, Width - 7, bar_size);
            pe.Graphics.FillRectangle(Brushes.White, barRect);
            pe.Graphics.DrawString(_last_volume.ToString() + "%", new Font("Tahoma", 7), Brushes.White, new PointF(0, 2));
        }
    }


}
