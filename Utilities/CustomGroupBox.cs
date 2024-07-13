using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace opentuner.Utilities
{
    // an attempt at making a collapsable group box :p

    public class CustomGroupBox : GroupBox
    {

        public string db_margin = "";

        public CustomGroupBox() 
        {
            this.SetStyle(ControlStyles.UserPaint, true);
        }



        protected override void OnPaint(PaintEventArgs e)
        {

            Graphics g = e.Graphics;

            Color borderColor = Color.FromKnownColor(KnownColor.ControlDark);
            Color textColor = Color.FromKnownColor(KnownColor.ControlText);
            Font textFont = this.Font;

            SizeF textSize = g.MeasureString(this.Text, textFont);

            int padding = (int)(textSize.Height / 2);

            Rectangle bounds = new Rectangle(0,  padding , this.Width, this.Height - (padding * 2));

            ControlPaint.DrawBorder(g, bounds, borderColor, ButtonBorderStyle.Dashed);

            Rectangle textBounds = new Rectangle(10, 0, (int)textSize.Width + 6, (int)textSize.Height);

            g.FillRectangle(new SolidBrush(Color.FromArgb(240, 240, 240)), textBounds);

            TextRenderer.DrawText(g, this.Text, textFont, new Point(10, 0), textColor);

            if (db_margin.Length > 1)
            {
                Font db_margin_font = new Font(this.Font.FontFamily, 15f, FontStyle.Bold);

                SizeF db_margin_textSize = g.MeasureString(db_margin.ToString(), db_margin_font);

                g.FillRectangle(new SolidBrush(Color.FromArgb(240, 240, 240)), new Rectangle(this.Width - (int)db_margin_textSize.Width - 15, 0, (int)db_margin_textSize.Width + 5, (int)db_margin_textSize.Height));
                TextRenderer.DrawText(g, db_margin.ToString(), db_margin_font, new Point(this.Width - (int)db_margin_textSize.Width - 15, 0), textColor);
            }
        }
    }
}
