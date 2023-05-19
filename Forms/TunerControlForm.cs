using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Input;
using opentuner.Hardware;

namespace opentuner.Forms
{
    public delegate void TunerChangeCallback(UInt32 freq, uint rf_input, uint symbol_rate);

    public partial class TunerControlForm : Form
    {
        private int frequency = 0;
        private int offset = 0;

        private int offset_a = 0;
        private int offset_b = 0;

        private TunerChangeCallback tuner_change;

        public TunerControlForm(TunerChangeCallback TunerChangeCallback)
        {
            InitializeComponent();
            lblkHz.MouseWheel += LblkHz_MouseWheel;
            lblmHz.MouseWheel += LblmHz_MouseWheel;
            lblgHz.MouseWheel += LblgHz_MouseWheel;
            comboRFInput.SelectedIndex = 0;
            tuner_change = TunerChangeCallback;
        }

        void scroll_frequency(int freq_modifier, int delta)
        {
            if (delta == 0)
                return;

            if (delta < 0 && frequency - freq_modifier >= 0)
            {
                frequency -= freq_modifier;
            }

            if (delta > 0 && ((frequency + freq_modifier) < 9999999))
            {
                frequency += freq_modifier;
            }

            update_freq(frequency);
        }
        private void LblgHz_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            scroll_frequency(1000000, e.Delta);
        }

        private void LblmHz_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            int modifier = 1000;

            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                modifier = 10000;

            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                modifier = 100000;


            scroll_frequency(modifier, e.Delta);
        }

        private void LblkHz_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            int modifier = 1;

            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                modifier = 10;

            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                modifier = 100;

            scroll_frequency(modifier, e.Delta);
        }

        private void update_freq(int new_freq)
        {
            int freq = Convert.ToInt32(new_freq) + offset;

            int kHz = 0;
            int mHz = 0;
            int gHz = 0;

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

        public void set_freq(TunerConfig newConfig)
        {
            frequency = Convert.ToInt32(newConfig.frequency);
            comboRFInput.SelectedIndex = (int)(newConfig.rf_input - 1);
            comboSR.Text = newConfig.symbol_rate.ToString();

            update_offset();
            update_freq(frequency);
        }

        public void set_offset(int A, int B)
        {
            offset_a = A;
            offset_b = B;

            update_offset();
        }

        private void update_offset()
        {
            if (comboRFInput.SelectedIndex == 0)
                offset = offset_a;
            else
                offset = offset_b;

            lblOffset.Text = offset.ToString();
        }

        private void tunerControlForm_Load(object sender, EventArgs e)
        {
            update_freq(frequency);
        }

        private void checkTunerOnTop_CheckedChanged(object sender, EventArgs e)
        {
            TopMost = checkTunerOnTop.Checked;
        }

        private void btnUpdateFreq_Click(object sender, EventArgs e)
        {
            if (tuner_change != null)
            {
                tuner_change(Convert.ToUInt32(frequency), Convert.ToUInt32(comboRFInput.SelectedIndex + 1), Convert.ToUInt32(comboSR.Text));
            }
        }

        private void comboRFInput_SelectedIndexChanged(object sender, EventArgs e)
        {
            update_offset();
        }

        private void tunerControlForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }
    }
}
