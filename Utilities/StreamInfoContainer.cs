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

        public StreamInfoContainer() 
        {
            BackColor = Color.Black;
            Dock = DockStyle.Top;
            Height = 30;
            Visible = false;
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
            Font = new Font("Arial", 12, FontStyle.Bold);

            pe.Graphics.FillRectangle(Brushes.Black, 0, 0, Width, Height);
            pe.Graphics.DrawRectangle(Pens.White, 0, 0, Width - 1, Height - 1);

            if (last_info_data == null)
                return;

            string info = "";

            string locked_info = last_info_data.service_name + " - " +
                " D" + last_info_data.db_margin.ToString("F1");

            info = 
                "MER: " + last_info_data.mer.ToString("F1") + " dB - " +
                last_info_data.frequency.ToString("N0", CultureInfo.InvariantCulture) +
                " - " + last_info_data.symbol_rate.ToString();

            int x_pos = 2;
            int y_pos = 5;

            if (last_info_data.demod_locked)
            {
                pe.Graphics.DrawString(locked_info, Font, Brushes.LimeGreen, new PointF(x_pos, y_pos));
                SizeF textSize = pe.Graphics.MeasureString(locked_info, Font);
                x_pos += (int)textSize.Width ;
            }
            else
            {
                pe.Graphics.DrawString("No Lock", Font, Brushes.PaleVioletRed, new PointF(x_pos, y_pos));
                SizeF textSize = pe.Graphics.MeasureString("No Lock", Font);
                x_pos += (int)textSize.Width;
            }

            pe.Graphics.DrawString(info, Font, Brushes.AntiqueWhite, new PointF(x_pos, y_pos));

            x_pos += (int)pe.Graphics.MeasureString(info, Font).Width;

            using (Font emojiFont = new Font("Segoe UI Emoji", 12, FontStyle.Bold))
            {
                string audio_icon_mute = "\U0001F507";
                string audio_icon_high = "\uD83D\uDD0A";

                if (last_info_data.volume > 0)
                    pe.Graphics.DrawString(audio_icon_high, emojiFont, Brushes.LimeGreen, new PointF(x_pos -5, 2));
                else
                    pe.Graphics.DrawString(audio_icon_mute, emojiFont, Brushes.IndianRed, new PointF(x_pos -5, 2));
            }



        }

    }
}
