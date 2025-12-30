using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using opentuner.MediaSources;
using System.Globalization;

namespace opentuner.Utilities
{
    public class StreamInfoContainer : Label
    {
        OTSourceData last_info_data = null;
        private Font font = new Font("Arial", 12, FontStyle.Bold);

        public StreamInfoContainer(bool show) 
        {
            BackColor = Color.Black;
            Dock = DockStyle.Top;

            //Height = 30;
            Visible = show;
        }

        public void UpdateInfo(OTSourceData info_data)
        {
            last_info_data = info_data;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);

            ForeColor = Color.Black;

            SizeF temp_text_size = pe.Graphics.MeasureString("Temp", font);
            this.Height = (int)temp_text_size.Height + 10;

            pe.Graphics.FillRectangle(Brushes.Black, 0, 0, Width, Height);
            pe.Graphics.DrawRectangle(Pens.White, 0, 0, Width - 1, Height - 1);

            if (last_info_data == null)
                return;

            string info = "";

            string locked_info = (last_info_data.service_name.Length == 0 ? "Lock" : last_info_data.service_name) + " - " + " D" + last_info_data.db_margin.ToString("F1");

            if (last_info_data.demod_locked)
            {
                info = "MER: " + last_info_data.mer.ToString("F1") + " dB";
            }
            info = info + " - " + last_info_data.frequency.ToString("N0", CultureInfo.CurrentCulture);
            if (last_info_data.demod_locked)
            {
                info = info + " - " + last_info_data.demode_state;
            }
            info = info + " - " + last_info_data.symbol_rate.ToString();
            if (last_info_data.demod_locked)
            {
                info = info + " - " + last_info_data.modcode;
            }

            int x_pos = 2;
            int y_pos = 5;

            if (last_info_data.demod_locked)
            {
                pe.Graphics.DrawString(locked_info, font, Brushes.LimeGreen, new PointF(x_pos, y_pos));
                SizeF textSize = pe.Graphics.MeasureString(locked_info, font);
                x_pos += (int)textSize.Width ;
            }
            else
            {
                pe.Graphics.DrawString("No Lock", font, Brushes.PaleVioletRed, new PointF(x_pos, y_pos));
                SizeF textSize = pe.Graphics.MeasureString("No Lock", font);
                x_pos += (int)textSize.Width;
            }

            pe.Graphics.DrawString(info, font, Brushes.AntiqueWhite, new PointF(x_pos, y_pos));

            x_pos += (int)pe.Graphics.MeasureString(info, font).Width;

            using (Font emojiFont = new Font("Segoe UI Emoji", 12, FontStyle.Bold))
            {
                string audio_icon_mute = "\U0001F507";
                string audio_icon_high = "\uD83D\uDD0A";

                if (last_info_data.volume > 0)
                    pe.Graphics.DrawString(audio_icon_high, emojiFont, Brushes.LimeGreen, new PointF(x_pos -5, 2));
                else
                    pe.Graphics.DrawString(audio_icon_mute, emojiFont, Brushes.IndianRed, new PointF(x_pos -5, 2));

                x_pos += (int)pe.Graphics.MeasureString(audio_icon_mute, emojiFont).Width;
            }

            if (last_info_data.streaming)
            {
                pe.Graphics.DrawString("U", font, Brushes.PaleTurquoise, new PointF(x_pos, y_pos));
                x_pos += (int)pe.Graphics.MeasureString("U ", font).Width;
            }

            if (last_info_data.recording)
            {
                pe.Graphics.DrawString("R", font, Brushes.PaleVioletRed, new PointF(x_pos, y_pos));
            }
        }
    }
}
