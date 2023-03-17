using LibVLCSharp.Shared;
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
using System.Xml.Linq;
using System.Globalization;
using WebSocketSharp;
using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.InteropServices;
//using NAudio.CoreAudioApi;
//using NAudio.CoreAudioApi.Interfaces;
//using CoreAudio;

namespace opentuner
{
    public partial class Form1 : Form
    {
        LibVLC libVLC = new LibVLC("--aout=directsound");
        Media media;
        TSStreamMediaInput mediaInput;

        ftdi ftdi_hw = null;
        bool hardware_connected = false;

        ConcurrentQueue<NimConfig> config_queue = new ConcurrentQueue<NimConfig>();
        ConcurrentQueue<NimStatus> ts_status_queue = new ConcurrentQueue<NimStatus>();
        ConcurrentQueue<byte> ts_data_queue = new ConcurrentQueue<byte>();
        ConcurrentQueue<byte> ts_parser_data_queue = new ConcurrentQueue<byte>();

        private delegate void updateNimStatusGuiDelegate(Form1 gui, NimStatus new_status);
        private delegate void updateTSStatusGuiDelegate(Form1 gui, TSStatus new_status);
        private delegate void updateMediaStatusGuiDelegate(Form1 gui, MediaStatus new_status);

        private delegate void UpdateLBDelegate(ListBox LB, Object obj);

        // threads
        Thread nim_thread_t = null;
        Thread ts_thread_t = null;
        Thread ts_parser_t = null;

        // form properties

        // nim status properties
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
        public string prop_db_margin { set { this.lbldbMargin.Text = value; } }
        public string prop_req_freq { set { this.lblReqFreq.Text = value; } }

        // ts status properties
        public string prop_service_name { set { this.lblServiceName.Text = value; } }
        public string prop_service_provider_name { set { this.lblServiceProvider.Text = value; } }
        public string prop_null_packets { set { this.lblNullPackets.Text = value; } }

        // media status properties
        public string prop_media_video_codec { set { this.lblVideoCodec.Text = value; } }
        public string prop_media_video_resolution { set { this.lblVideoResolution.Text = value; } }
        public string prop_media_audio_codec { set { this.lblAudioCodec.Text = value; } }
        public string prop_media_audio_rate { set { this.lblAudioRate.Text = value; } }


        // quick tune variables *********************************************************************
        private static readonly Object list_lock = new Object();

        static int width = 1500;     //web monitor uses 922 points, 6 padded?
        static int height = 255;    //makes things easier
        static int bandplan_height = 30;

        Bitmap bmp;
        static Bitmap bmp2;
        Pen greenpen = new Pen(Color.FromArgb(200, 20, 200, 20));
        //Pen greenpen = new Pen(Color.FromArgb(250, 0, 0, 200));
        SolidBrush shadowBrush = new SolidBrush(Color.FromArgb(128, Color.Gray));
        SolidBrush bandplanBrush = new SolidBrush(Color.FromArgb(180, 250, 250, 255));
        SolidBrush overpowerBrush = new SolidBrush(Color.FromArgb(128, Color.Red));

        Graphics tmp;
        Graphics tmp2;

        int[] rx_blocks = new int[3];

        double start_freq = 10490.5f;

        XElement bandplan;
        Rectangle[] channels;
        IList<XElement> indexedbandplan;
        string InfoText;
        List<string> blocks = new List<string>();

        socket sock;
        signal sigs;

        int num_rxs_to_scan = 1;

        bool ts_build_queue_flag = false;
        bool prevLocked = false;

        uint current_frequency = 0;

        public static void UpdateLB(ListBox LB, Object obj)
        {
            if (LB.InvokeRequired)
            {
                UpdateLBDelegate ulb = new UpdateLBDelegate(UpdateLB);
                LB.Invoke(ulb, new object[] { LB, obj });
            }
            else
            {
                if (LB.Items.Count > 1000)
                {
                    LB.Items.Remove(0);
                }

                int i = LB.Items.Add(DateTime.Now.ToShortTimeString() + " : " + obj);
                LB.TopIndex = i;
            }

        }

        private void debug(string msg)
        {
            UpdateLB(dbgListBox, msg);
        }

        public static void updateMediaStatusGui(Form1 gui, MediaStatus new_status)
        {
            if (gui.InvokeRequired)
            {
                updateMediaStatusGuiDelegate del = new updateMediaStatusGuiDelegate(updateMediaStatusGui);
                gui.Invoke(del, new object[] { gui, new_status });
            }
            else
            {
                gui.prop_media_video_codec = new_status.VideoCodec;
                gui.prop_media_video_resolution = new_status.VideoWidth.ToString() + " x " + new_status.VideoHeight.ToString();

                gui.prop_media_audio_codec = new_status.AudioCodec;
                gui.prop_media_audio_rate = new_status.AudioRate.ToString() + " Hz, " + new_status.AudioChannels.ToString() + " channels";
            }

        }


        public static void updateTSStatusGui(Form1 gui, TSStatus new_status)
        {
            if (gui.InvokeRequired)
            {
                updateTSStatusGuiDelegate del = new updateTSStatusGuiDelegate(updateTSStatusGui);
                gui.Invoke(del, new object[] { gui, new_status });
            }
            else
            {
                gui.prop_service_name = new_status.ServiceName;
                gui.prop_service_provider_name = new_status.ServiceProvider;
                gui.prop_null_packets = new_status.NullPacketsPerc.ToString() + "%";
            }

        }

        public  void updateNimStatusGui(Form1 gui, NimStatus new_status)
        {
            if ( gui.InvokeRequired )
            {
                updateNimStatusGuiDelegate del = new updateNimStatusGuiDelegate(updateNimStatusGui);
                gui.Invoke(del, new object[] { gui, new_status });
            }
            else
            {
                gui.prop_demodstate = lookups.demod_state_lookup[new_status.demod_status];
                double mer = new_status.mer / 10;
                gui.prop_mer = mer.ToString() + " dB";
                gui.prop_lnagain = new_status.lna_gain.ToString();
                gui.prop_power_i = new_status.power_i.ToString();
                gui.prop_power_q = new_status.power_q.ToString();
                gui.prop_symbol_rate = new_status.symbol_rate.ToString();
                gui.prop_modcod = new_status.modcode.ToString();
                gui.prop_lpdc_errors = new_status.errors_ldpc_count.ToString();
                gui.prop_ber = new_status.ber.ToString();
                gui.prop_freq_carrier_offset = new_status.frequency_carrier_offset.ToString();


                gui.prop_req_freq = current_frequency.ToString();

                double dbmargin = 0;

                if ( new_status.demod_status < 2 )
                {
                    gui.prop_service_provider_name = "";
                    gui.prop_service_name = "";
                    gui.prop_null_packets = "";

                    gui.prop_media_audio_rate = "";
                    gui.prop_media_video_codec = "";
                    gui.prop_media_video_resolution = "";
                    gui.prop_media_audio_codec = "";
                }

                try
                {
                    switch (new_status.demod_status)
                    {
                        case 2:
                            gui.prop_modcod = lookups.modcod_lookup_dvbs2[new_status.modcode];
                            dbmargin = (mer - lookups.modcod_lookup_dvbs2_threshold[new_status.modcode]);
                            gui.prop_db_margin = "D" + dbmargin.ToString("N1");
                            break;
                        case 3:
                            gui.prop_modcod = lookups.modcod_lookup_dvbs[new_status.modcode];
                            dbmargin = (mer - lookups.modcod_lookup_dvbs_threshold[new_status.modcode]);
                            gui.prop_db_margin = "D" + dbmargin.ToString("N1");
                            break;
                        default:
                            gui.prop_modcod = "Unknown";
                            gui.prop_db_margin = "";
                            break;
                    }
                }
                catch (Exception Ex)
                {
                }

            }

        }
        public Form1()
        {
            InitializeComponent();
        }

        public void start_video()
        {
            Console.WriteLine("Main: Starting VLC");


            if (videoView1.MediaPlayer != null)
                videoView1.MediaPlayer.Play(media);
        }

        public void stop_video()
        {
            Console.WriteLine("Main: Stopping VLC");

            if (videoView1.MediaPlayer != null)
                videoView1.MediaPlayer.Stop();

           
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

        private void MediaPlayer_EncounteredError(object sender, EventArgs e)
        {
            Console.WriteLine("VLC: Error: " + e.ToString());
        }

        private void MediaPlayer_Playing(object sender, EventArgs e)
        {
            Console.WriteLine("VLC: Playing ");
        }

        private void MediaPlayer_Stopped(object sender, EventArgs e)
        {
            Console.WriteLine("VLC: Stopped");
        }

        public void parse_ts_data_callback(TSStatus ts_status)
        {
            updateTSStatusGui(this, ts_status);
        }

        public void nim_status_feedback(NimStatus nim_status)
        {
            bool locked = false;

            if (nim_status.demod_status >= 2) locked = true;

            if (prevLocked != locked)
            {
                Console.WriteLine("Lock State Change: " + prevLocked.ToString() + "->" + locked.ToString());

                if (nim_status.demod_status >= 2)
                {
                    Console.WriteLine("Startng TS queue and VLC");
                    nim_status.build_queue = true;
                    ts_build_queue_flag = true;
                    start_video();
                }
                else
                {
                    Console.WriteLine("Stopping TS queue and VLC");
                    nim_status.build_queue = false;
                    ts_build_queue_flag = false;
                    stop_video();
                }

                prevLocked = locked;
            }
            else
            {
                nim_status.build_queue = ts_build_queue_flag;
            }
            
            updateNimStatusGui(this, nim_status);

            // inform ts thread of whats happening
            ts_status_queue.Enqueue(nim_status);

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            stop_video();

            if (mediaInput != null)
                mediaInput.Dispose();
            if (media != null)
                media.Dispose();

            if (nim_thread_t != null)
                nim_thread_t.Abort();
            if (ts_thread_t != null)
                ts_thread_t.Abort();
            if (ts_parser_t != null)
                ts_parser_t.Abort();

        }

        private void btnConnectTuner_Click(object sender, EventArgs e)
        {
            hardware_init();

            if (!hardware_connected)
            {
                MessageBox.Show("Error: No Working Hardware Detected");
                return;
            }

            Console.WriteLine("Main: Starting Nim Thread");

            // NIM thread
            NimStatusCallback status_callback = new NimStatusCallback(nim_status_feedback);

            NimThread nim_thread = new NimThread(config_queue, ftdi_hw, status_callback);

            nim_thread_t = new Thread(nim_thread.worker_thread);

            NimConfig initialConfig = new NimConfig();
            initialConfig.frequency = 741525;
            initialConfig.symbol_rate = 1500;

            current_frequency = initialConfig.frequency;

            // we need to make sure we have a config queued before starting the thread
            config_queue.Enqueue(initialConfig);

            nim_thread_t.Start();

            Console.WriteLine("Main: Starting TS Thread");

            // TS thread

            TSThread ts_thread = new TSThread(ftdi_hw, ts_status_queue, ts_data_queue, ts_parser_data_queue);
            ts_thread_t = new Thread(ts_thread.worker_thread);
            ts_thread_t.Start();

            // TS Parser Thread
            TSDataCallback ts_data_callback = new TSDataCallback(parse_ts_data_callback);
            TSParserThread ts_parser_thread = new TSParserThread(ts_data_callback, ts_parser_data_queue);
            ts_parser_t = new Thread(ts_parser_thread.worker_thread);
            ts_parser_t.Start();

            videoView1.MediaPlayer = new MediaPlayer(libVLC);
            videoView1.MediaPlayer.Stopped += MediaPlayer_Stopped;
            videoView1.MediaPlayer.Playing += MediaPlayer_Playing;
            videoView1.MediaPlayer.EncounteredError += MediaPlayer_EncounteredError;
            videoView1.MediaPlayer.Vout += MediaPlayer_Vout;
            

            mediaInput = new TSStreamMediaInput(ts_data_queue);
            media = new Media(libVLC, mediaInput);

            MediaConfiguration mediaConfig = new MediaConfiguration();
            mediaConfig.EnableHardwareDecoding = false;
            media.AddOption(mediaConfig);

            // temporary to prevent multiple connection attempts
            // todo: deal with this properly
            lblConnected.Text = "Connected";
            lblConnected.ForeColor = Color.Green;

            btnConnectTuner.Enabled = false;

        }

        private void MediaPlayer_Vout(object sender, MediaPlayerVoutEventArgs e)
        {
            MediaStatus media_status = new MediaStatus();

            foreach ( var track in media.Tracks)
            {
                switch(track.TrackType)
                {
                    case TrackType.Audio:
                        media_status.AudioChannels = track.Data.Audio.Channels;
                        media_status.AudioCodec = media.CodecDescription(TrackType.Audio, track.Codec);
                        media_status.AudioRate = track.Data.Audio.Rate;
                        break;
                    case TrackType.Video:
                        media_status.VideoCodec = media.CodecDescription(TrackType.Video, track.Codec);
                        media_status.VideoWidth = track.Data.Video.Width;
                        media_status.VideoHeight = track.Data.Video.Height;
                        break;
                }
            }

            updateMediaStatusGui(this, media_status);
        }

        private void btnFrequencyChange_Click(object sender, EventArgs e)
        {
            UInt32 freq = Convert.ToUInt32(txtFreq.Text);
            UInt32 lo = Convert.ToUInt32(txtLO.Text);
            UInt32 sr = Convert.ToUInt32(txtSR.Text);

            change_frequency(freq, lo, sr);
        }

        void change_frequency(UInt32 freq, UInt32 lo, UInt32 sr)
        {
            NimConfig newConfig = new NimConfig();

            newConfig.frequency = freq - lo;
            newConfig.symbol_rate = sr;

            debug("Main: New Config: " + newConfig.ToString());

            current_frequency = newConfig.frequency;

            config_queue.Enqueue(newConfig);
        }

        // quicktune functions
        private void drawspectrum_bandplan()
        {
            int span = 9;
            int count = 0;

            float spectrum_w = spectrum.Width;
            float spectrum_wScale = spectrum_w / 922;

            List<string> blocks = new List<string>();

            //count blocks ('layers' of bandplan)
            foreach (var channel in bandplan.Elements("channel"))
            {
                count++;
                if (!blocks.Contains(channel.Element("block").Value))
                {
                    blocks.Add(channel.Element("block").Value);
                }
            }

            channels = new Rectangle[count];

            int n = 0;

            //create rectangle blocks to display bandplan
            foreach (var channel in bandplan.Elements("channel"))
            {
                int w = 0;
                int offset = 0;
                float rolloff = 1.35f;
                string xval = channel.Element("x-freq").Value;

                float freq;
                int sr;

                freq = Convert.ToSingle(xval, CultureInfo.InvariantCulture);
                sr = Convert.ToInt32(channel.Element("sr").Value, CultureInfo.InvariantCulture);

                int pos = Convert.ToInt16((922.0 / span) * (freq - start_freq));
                w = Convert.ToInt32(sr / (span * 1000.0) * 922 * rolloff);
                w = Convert.ToInt32(w * spectrum_wScale);

                int split = bandplan_height / blocks.Count();
                int b = blocks.Count();
                foreach (string blk in blocks)
                {
                    if (channel.Element("block").Value == blk)
                    {
                        offset = b * split;
                    }
                    b--;
                }
                channels[n] = new Rectangle(Convert.ToInt32(pos * spectrum_wScale) - (w / 2), offset - (split / 2) - 3, w, split - 2);
                n++;
            }

            //draw blocks
            for (int i = 0; i < count; i++)
            {
                tmp2.FillRectangles(bandplanBrush, new RectangleF[] { channels[i] });      //x,y,w,h
            }
        }

        private void drawspectrum_signals(List<signal.Sig> signals)
        {
            float spectrum_w = spectrum.Width;
            float spectrum_wScale = spectrum_w / 922;

            lock (list_lock)        //hopefully lock signals list while drawing
            {
                //draw the text for each signal found
                foreach (signal.Sig s in signals)
                {
                    tmp.DrawString(s.callsign + "\n" + s.frequency.ToString("#.00") + "\n " + (s.sr * 1000).ToString("#Ks"), new Font("Tahoma", 10), Brushes.White, new PointF(Convert.ToSingle((s.fft_centre * spectrum_wScale) - (25)), (255 - Convert.ToSingle(s.fft_strength + 50))));
                }
            }
            try
            {
                this.Invoke(new MethodInvoker(delegate () { spectrum.Image = bmp; spectrum.Update(); }));
            }
            catch (Exception Ex)
            {

            }
        }
        private void drawspectrum(UInt16[] fft_data)
        {
            tmp.Clear(Color.Black);     //clear canvas

            int spectrum_h = spectrum.Height - bandplan_height;
            float spectrum_w = spectrum.Width;
            float spectrum_wScale = spectrum_w / 922;

            PointF[] points = new PointF[fft_data.Length - 2];

            int i = 1;

            for (i = 1; i < fft_data.Length - 3; i++)     //ignore padding?
            {
                // tmp.DrawLine(greenpen, i - 1, 255 - fft_data[i - 1] / 255, i, 255 - fft_data[i] / 255);
                PointF point = new PointF(i * spectrum_wScale, 255 - fft_data[i] / 255);
                points[i] = point;
            }

            points[0] = new PointF(0, 255);
            points[points.Length - 1] = new PointF(spectrum_w, 255);

            //tmp.DrawPolygon(greenpen, points);
            SolidBrush spectrumBrush = new SolidBrush(Color.Blue);

            System.Drawing.Drawing2D.LinearGradientBrush linGrBrush = new LinearGradientBrush(
               new Point(0, 0),
               new Point(0, 255),
               Color.FromArgb(255, 255, 99, 132),   // Opaque red
               Color.FromArgb(255, 54, 162, 235));  // Opaque blue

            tmp.FillPolygon(linGrBrush, points);

            tmp.DrawImage(bmp2, 0, 255 - bandplan_height); //bandplan

            //draw block showing signal selected
            tmp.FillRectangles(shadowBrush, new RectangleF[] { new System.Drawing.Rectangle(Convert.ToInt32((rx_blocks[0] * spectrum_wScale) - ((rx_blocks[1] * spectrum_wScale) / 2)), 1, Convert.ToInt32(rx_blocks[1] * spectrum_wScale), (255) - 4) });

            tmp.DrawString(InfoText, new Font("Tahoma", 15), Brushes.White, new PointF(10, 10));


            //drawspectrum_signals(sigs.detect_signals(fft_data));
            sigs.detect_signals(fft_data);

            /*
            // draw over power
            foreach (var sig in sigs.signalsData)
            {
                if ( sig.overpower )
                    tmp.FillRectangles(overpowerBrush, new RectangleF[] { new System.Drawing.Rectangle(Convert.ToInt16(sig.fft_centre) - (Convert.ToInt16(sig.fft_stop-sig.fft_start) / 2), 1, Convert.ToInt16(sig.fft_stop-sig.fft_start), (255) - 4) });
            }
            */

            drawspectrum_signals(sigs.signalsData);
        }

        private void spectrum_Click(object sender, EventArgs e)
        {

            float spectrum_w = spectrum.Width;
            float spectrum_wScale = spectrum_w / 922;

            MouseEventArgs me = (MouseEventArgs)e;
            var pos = me.Location;


            int X = pos.X;
            int Y = pos.Y;

            if (me.Button == MouseButtons.Right)
            {
                int freq = Convert.ToInt32((10490.5 + ((X / spectrum_wScale) / 922.0) * 9.0) * 1000.0);
                //UpdateTextBox(txtFreq, freq.ToString());
            }
            else
            {
                selectSignal(X);
            }

        }


        // quick tune functions - From https://github.com/m0dts/QO-100-WB-Live-Tune - Rob Swinbank
        private void selectSignal(int X)
        {

            float spectrum_w = spectrum.Width;
            float spectrum_wScale = spectrum_w / 922;

            debug("Select Signal");
            try
            {
                foreach (signal.Sig s in sigs.signals)
                {
                    if ((X / spectrum_wScale) > s.fft_start & (X / spectrum_wScale) < s.fft_stop)
                    {

                        sigs.set_tuned(s, 0);
                        rx_blocks[0] = Convert.ToInt16(s.fft_centre);
                        rx_blocks[1] = Convert.ToInt16((s.fft_stop) - (s.fft_start));
                        UInt32 freq = Convert.ToUInt32((s.frequency) * 1000);
                        UInt32 sr = Convert.ToUInt32((s.sr * 1000.0));

                        debug("Freq: " + freq.ToString());
                        debug("SR: " + sr.ToString());


                        UInt32 lo = Convert.ToUInt32(txtLO.Text);

                        change_frequency(freq, lo, sr);

                    }
                }
            }
            catch (Exception Ex)
            {

            }
        }

        public void spectrum_MouseMove(object sender, MouseEventArgs e)
        {
            //detect mouse over channel, tooltip info
            int n = 0;
            if (e.Y > (spectrum.Height - bandplan_height))
            {
                if (channels != null)
                {
                    foreach (Rectangle ch in channels)
                    {
                        if (e.X >= ch.Location.X & e.X <= ch.Location.X + ch.Width)
                        {
                            if (e.Y - (spectrum.Height - bandplan_height) >= ch.Location.Y - (ch.Height / 2) + 3 & e.Y - (spectrum.Height - bandplan_height) <= ch.Location.Y + (ch.Height / 2) + 3)
                            {
                                InfoText = "SR: " + indexedbandplan[n].Element("name").Value + " Dn: " + indexedbandplan[n].Element("x-freq").Value + " Up: " + indexedbandplan[n].Element("s-freq").Value;
                            }

                        }
                        n++;
                    }
                }
            }
            else
            {
                if (InfoText != "")
                {
                    InfoText = "";
                }
            }

        }


        private void webSocketTimeout_Tick(object sender, EventArgs e)
        {
            if (sock != null)
            {
                TimeSpan t = DateTime.Now - sock.lastdata;

                if (t.Seconds > 2)
                {
                    debug("FFT Websocket Timeout, Disconnected");
                    sock.stop();
                }

                if (!sock.connected)
                {
                    sock.start();
                }
            }

        }

        public float align_symbolrate(float width)
        {
            if (width < 0.022)
            {
                return 0;
            }
            else if (width < 0.065)
            {
                return 0.035f;
            }
            else if (width < 0.086)
            {
                return 0.066f;
            }
            else if (width < 0.195)
            {
                return 0.125f;
            }
            else if (width < 0.277)
            {
                return 0.250f;
            }
            else if (width < 0.388)
            {
                return 0.333f;
            }
            else if (width < 0.700)
            {
                return 0.500f;
            }
            else if (width < 1.2)
            {
                return 1.000f;
            }
            else if (width < 1.6)
            {
                return 1.500f;
            }
            else if (width < 2.2)
            {
                return 2.000f;
            }
            else
            {
                return Convert.ToSingle(Math.Round(width * 5) / 5.0);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            bmp2 = new Bitmap(spectrum.Width, bandplan_height);     //bandplan
            bmp = new Bitmap(spectrum.Width, height + 20);
            tmp = Graphics.FromImage(bmp);
            tmp2 = Graphics.FromImage(bmp2);

            try
            {
                bandplan = XElement.Load(Path.GetDirectoryName(Application.ExecutablePath) + @"\bandplan.xml");
                drawspectrum_bandplan();
                indexedbandplan = bandplan.Elements().ToList();
                foreach (var channel in bandplan.Elements("channel"))
                {
                    if (!blocks.Contains(channel.Element("block").Value))
                    {
                        blocks.Add(channel.Element("block").Value);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            sock = new socket();
            sigs = new signal(list_lock);
            sock.callback += drawspectrum;
            sigs.debug += debug;
            string title = this.Text;
            sock.start();
            this.Text = title;

            this.DoubleBuffered = true;

            sigs.set_num_rx_scan(num_rxs_to_scan);
            sigs.set_num_rx(1);
            //sigs.set_avoidbeacon(avoidBeacon);
            sigs.set_avoidbeacon(true);

        }

        private void websocketTimer_Tick(object sender, EventArgs e)
        {
            if (sock != null)
            {
                TimeSpan t = DateTime.Now - sock.lastdata;

                if (t.Seconds > 2)
                {
                    debug("FFT Websocket Timeout, Disconnected");
                    sock.stop();
                }

                if (!sock.connected)
                {
                    sock.start();
                }
            }

        }

        private void spectrum_SizeChanged(object sender, EventArgs e)
        {
            try
            {
                bmp2 = new Bitmap(spectrum.Width, bandplan_height);     //bandplan
                bmp = new Bitmap(spectrum.Width, height + 20);
                tmp = Graphics.FromImage(bmp);
                tmp2 = Graphics.FromImage(bmp2);

                drawspectrum_bandplan();
            }
            catch (Exception Ex)
            {

            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.zr6tg.co.za/open-tuner/");
        }
    }


}
