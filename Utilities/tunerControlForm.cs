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
using System.Windows.Input;
using Vortice.MediaFoundation;

namespace opentuner
{
    public delegate void TunerChangeCallback(int id, UInt32 freq);

    public delegate void TunerDataUpdateDelegate(uint freq, uint symbolrate, uint offset);

    public partial class TunerControlForm : Form
    {
        private uint _frequency = 0;
        private uint _symbol_rate = 0;
        private uint _offset = 0;

        private int _id;

        public event TunerChangeCallback OnTunerChange;

        OTSource _source = null;

        public TunerControlForm(int Id, uint initialFrequency, uint initialSr, uint Offset, OTSource Source)
        {
            InitializeComponent();

            _id = Id;
            _frequency = initialFrequency;
            _symbol_rate = initialSr;
            _offset = Offset;

            _source = Source;

            update_offset();
            update_freq(_frequency);
            update_sr(_symbol_rate);


            lblkHz.MouseWheel += LblkHz_MouseWheel;
            lblmHz.MouseWheel += LblmHz_MouseWheel;
            lblgHz.MouseWheel += LblgHz_MouseWheel;

            this.Text += " - Tuner " + (Id + 1).ToString();
        }

        void scroll_frequency(uint freq_modifier, int delta)
        {
            if (delta == 0)
                return;

            if (delta < 0 && _frequency - freq_modifier >= 0)
            {
                _frequency -= freq_modifier;
            }

            if (delta > 0 && ((_frequency + freq_modifier) < 9999999))
            {
                _frequency += freq_modifier;
            }

            update_freq(_frequency);
        }

        private void LblgHz_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            scroll_frequency(1000000, e.Delta);
        }

        private void LblmHz_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            uint modifier = 1000;

            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                modifier = 10000;

            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                modifier = 100000;


            scroll_frequency(modifier, e.Delta);
        }

        private void LblkHz_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            uint modifier = 1;

            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                modifier = 10;

            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                modifier = 100;

            scroll_frequency(modifier, e.Delta);
        }

        private void update_freq(uint new_freq)
        {
            uint freq = new_freq + _offset;

            uint kHz = 0;
            uint mHz = 0;
            uint gHz = 0;

            gHz = freq / 1000000;
            mHz = (freq - (gHz * 1000000)) / 1000;
            kHz = freq - (gHz * 1000000) - (mHz * 1000);

            lblgHz.Text = gHz.ToString().PadLeft(3, '0');
            lblmHz.Text = mHz.ToString().PadLeft(3, '0');
            lblkHz.Text = kHz.ToString().PadLeft(3, '0');

            lblNimFreq.Text = new_freq.ToString() + " kHz";

            if (new_freq < 144000 || new_freq > 2450000)
            {
                lblNimFreq.ForeColor = Color.Red;
                btnUpdateFreq.Enabled = false;
            }
            else
            {
                lblNimFreq.ForeColor = Color.Black;
                btnUpdateFreq.Enabled = true;
            }
        }

        private void update_offset()
        {
            lblOffset.Text = _offset.ToString();
        }

        private void tunerControlForm_Load(object sender, EventArgs e)
        {
            update_freq(_frequency);
        }

        private void checkTunerOnTop_CheckedChanged(object sender, EventArgs e)
        {
            TopMost = checkTunerOnTop.Checked;
        }

        private void btnUpdateFreq_Click(object sender, EventArgs e)
        {
            OnTunerChange?.Invoke(_id, Convert.ToUInt32(_frequency));
        }

        private void update_sr(uint symbolrate)
        {
            lblSR.Text = symbolrate.ToString();
        }

        private void update_offset(uint offset)
        {
            lblOffset.Text = offset.ToString();
        }

        private void UpdateTunerInvoke(uint freq, uint symbolrate, uint offset)
        {
            _frequency = freq;
            _symbol_rate = symbolrate;
            _offset = offset;

            update_sr(_symbol_rate);
            update_offset(_offset);

            update_freq(_frequency);

        }

        public void UpdateTuner(uint freq, uint symbolrate, uint offset)
        {
            if (this.InvokeRequired)
            {
                TunerDataUpdateDelegate ulb = new TunerDataUpdateDelegate(UpdateTuner);
                if (ulb != null)
                {
                    this.Invoke(ulb, new object[] { freq, symbolrate, offset });
                }
            }
            else
            {
                UpdateTunerInvoke(freq, symbolrate, offset);
            }
        }

        public void ShowTuner(uint freq, uint symbolrate, uint offset)
        {
            _frequency = freq;
            _symbol_rate = symbolrate;
            _offset = offset;

            update_sr(_symbol_rate);
            update_offset(_offset);

            update_freq(_frequency);

            Show();
            Focus();
        }


        private void lblkHz_MouseEnter(object sender, EventArgs e)
        {
            lblkHz.ForeColor = Color.DarkGreen;
        }

        private void lblkHz_MouseLeave(object sender, EventArgs e)
        {
            lblkHz.ForeColor = Color.Black;
        }

        private void lblmHz_MouseEnter(object sender, EventArgs e)
        {
            lblmHz.ForeColor = Color.DarkGreen;
        }

        private void lblmHz_MouseLeave(object sender, EventArgs e)
        {
            lblmHz.ForeColor = Color.Black;
        }

        private void lblgHz_MouseEnter(object sender, EventArgs e)
        {
            lblgHz.ForeColor = Color.DarkGreen;
        }

        private void lblgHz_MouseLeave(object sender, EventArgs e)
        {
            lblgHz.ForeColor = Color.Black;
        }

        private void TunerControlForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }
    }
}
