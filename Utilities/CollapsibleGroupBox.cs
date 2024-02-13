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

    public class CollapsibleGroupBox : GroupBox
    {
        private Rectangle _collapse_button;

        private bool _collapsed = false;

        public CollapsibleGroupBox() 
        {
            this.MouseDown += CustomGroupBox_MouseDown;
        }

        private void CustomGroupBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (_collapse_button.Contains(e.Location))
            {
                if (!_collapsed)
                {
                    this.AutoSize = false;
                    this.Height = 20;
                    _collapsed = true;
                }
                else
                {
                    this.AutoSize = true;
                    _collapsed = false;
                }
                
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            int squareSize = 15;
            _collapse_button = new Rectangle(
                this.Width - squareSize-2,
                2,
                squareSize,
                squareSize);

            e.Graphics.FillRectangle(new SolidBrush(Color.White), _collapse_button);
            e.Graphics.DrawRectangle(new Pen(Color.Black), _collapse_button);
        }
    }
}
