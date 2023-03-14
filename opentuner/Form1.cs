using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace opentuner
{
    public partial class Form1 : Form
    {
        ftdi ftdi_hw = null;
        bool hardware_connected = false;

        ConcurrentQueue<NimConfig> config_queue = new ConcurrentQueue<NimConfig>();
        ConcurrentQueue<NimStatus> ts_status_queue = new ConcurrentQueue<NimStatus>();

        private delegate void updateStatusGuiDelegate(Form1 gui, NimStatus new_status);

        // threads
        Thread nim_thread_t = null;
        Thread ts_thread_t = null;

        // form properties
        public string prop_demodstate { set { this.lblDemoState.Text = value; } }
        public string prop_mer { set { this.lblMer.Text = value; } }
        public string prop_lnagain { set { this.lblLnaGain.Text = value; } }
        public string prop_power_i { set { this.lblpower_i.Text = value; } }
        public string prop_power_q { set { this.lblPower_q.Text = value; } }
        public string prop_symbol_rate { set { this.lblSR.Text = value; } }
        public string prop_modcod { set { this.lblModcod.Text = value; } }
        public string prop_lpdc_errors { set { this.lblLPDCError.Text = value; } }
        public string prop_ber { set { this.lblBer.Text = value; } }
        public string prop_freq_carrier_offset { set { this.lblFreqCar.Text = value; } }



        public static void updateStatusGui(Form1 gui, NimStatus new_status)
        {
            if ( gui.InvokeRequired )
            {
                updateStatusGuiDelegate del = new updateStatusGuiDelegate(updateStatusGui);
                gui.Invoke(del, new object[] { gui, new_status });
            }
            else
            {
                gui.prop_demodstate = new_status.demod_status.ToString();
                gui.prop_mer = (new_status.mer/10).ToString();
                gui.prop_lnagain = new_status.lna_gain.ToString();
                gui.prop_power_i = new_status.power_i.ToString();
                gui.prop_power_q = new_status.power_q.ToString();
                gui.prop_symbol_rate = new_status.symbol_rate.ToString();
                gui.prop_modcod = new_status.modcode.ToString();
                gui.prop_lpdc_errors = new_status.errors_ldpc_count.ToString();
                gui.prop_ber = new_status.ber.ToString();
                gui.prop_freq_carrier_offset = new_status.frequency_carrier_offset.ToString();
            }

        }
        public Form1()
        {
            InitializeComponent();
        }

        private void hardware_init()
        {
            ftdi_hw = new ftdi();
            byte err = ftdi_hw.ftdi_init();

            if (err != 0)
            {
                Console.WriteLine("Main: Error: FTDI Failed " + err.ToString());
                hardware_connected = false;
                return;
            }


            hardware_connected = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            hardware_init();

            if (!hardware_connected)
            {
                Console.WriteLine("Main: Error: No Working Hardware Detected");
                return;
            }

            Console.WriteLine("Main: Starting Nim Thread");

            // NIM thread
            NimStatusCallback status_callback = new NimStatusCallback(nim_status_feedback);

            NimThread nim_thread = new NimThread(config_queue, ftdi_hw, status_callback);

            nim_thread_t = new Thread(nim_thread.worker_thread);

            NimConfig initialConfig = new NimConfig();
            initialConfig.frequency = 745490;
            initialConfig.symbol_rate = 333;

            // we need to make sure we have a config queued before starting the thread
            config_queue.Enqueue(initialConfig);

            nim_thread_t.Start();

            Console.WriteLine("Main: Starting TS Thread");

            // TS thread
            TSThread ts_thread = new TSThread(ftdi_hw, ts_status_queue);
            ts_thread_t = new Thread(ts_thread.worker_thread);
            ts_thread_t.Start();
        }

        public void nim_status_feedback(NimStatus nim_status)
        {
            updateStatusGui(this, nim_status);

            if (nim_status.reset)
                ts_status_queue.Enqueue(nim_status);
        }

        private void button2_Click(object sender, EventArgs e)
        {



        }

        private void button4_Click(object sender, EventArgs e)
        {
            NimConfig initialConfig = new NimConfig();
            UInt32 freq = Convert.ToUInt32(txtFreq.Text);
            UInt32 lo = Convert.ToUInt32(txtLO.Text);
            UInt32 sr = Convert.ToUInt32(txtSR.Text);

            initialConfig.frequency = freq - lo;
            initialConfig.symbol_rate = sr;

            Console.WriteLine("Main: New Config: " + initialConfig.ToString());

            // we need to make sure we have a config queued before starting the thread
            config_queue.Enqueue(initialConfig);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (nim_thread_t != null)
                nim_thread_t.Abort();
            if (ts_thread_t != null)
                ts_thread_t.Abort();
        }
    }
}
