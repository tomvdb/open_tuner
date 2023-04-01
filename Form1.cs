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
using Newtonsoft.Json;

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
        //ConcurrentQueue<NimStatus> ts_status_queue = new ConcurrentQueue<NimStatus>();
        ConcurrentQueue<byte> ts_data_queue = new ConcurrentQueue<byte>();
        ConcurrentQueue<byte> ts_parser_data_queue = new ConcurrentQueue<byte>();

        private delegate void updateNimStatusGuiDelegate(Form1 gui, NimStatus new_status);
        private delegate void updateTSStatusGuiDelegate(Form1 gui, TSStatus new_status);
        private delegate void updateMediaStatusGuiDelegate(Form1 gui, MediaStatus new_status);
        private delegate void UpdateLBDelegate(ListBox LB, Object obj);
        private delegate void updateRecordingStatusDelegate(Form1 gui, bool recording_status);

        // threads
        Thread nim_thread_t = null;
        Thread ts_thread_t = null;
        Thread ts_parser_t = null;
        Thread ts_recorder_t = null;
        Thread ts_udp_t = null;

        TSRecorderThread ts_recorder;
        TSUDPThread ts_udp;

        // form properties
        public MediaPlayer prop_media_player { get { return this.videoView1.MediaPlayer; } }

        // nim status properties
        public string prop_demodstate { set { this.lblDemoState.Text = value; } }
        public string prop_mer { set { this.lblMer.Text = value; } get { return this.lblMer.Text; } }
        public string prop_lnagain { set { this.lblLnaGain.Text = value; } }
        public string prop_power_i { set { /*this.lblpower_i.Text = value;*/ } }
        public string prop_power_q { set { /*this.lblPower_q.Text = value;*/ } }

        public string prop_rf_input_level { set { this.lblRFInputLevel.Text = value; } }

        public string prop_symbol_rate { set { this.lblSR.Text = value; } }
        public string prop_modcod { set { this.lblModcod.Text = value; } get { return this.lblModcod.Text; } }
        public string prop_lpdc_errors { set { this.lblLPDCError.Text = value; } }
        public string prop_ber { set { this.lblBer.Text = value; } }
        public string prop_freq_carrier_offset { set { this.lblFreqCar.Text = value; } }
        public string prop_db_margin { set { this.lbldbMargin.Text = value; } get { return this.lbldbMargin.Text; } }
        public string prop_req_freq { set { this.lblReqFreq.Text = value; } }

        // ts status properties
        public string prop_service_name { set { this.lblServiceName.Text = value; } get { return this.lblServiceName.Text;  } }
        public string prop_service_provider_name { set { this.lblServiceProvider.Text = value; } get { return this.lblServiceProvider.Text;  } }
        public string prop_null_packets { set { this.lblNullPackets.Text = value; }  }
        public int prop_null_packets_bar { set { this.nullPacketsBar.Value = value; } }

        // media status properties
        public string prop_media_video_codec { set { this.lblVideoCodec.Text = value; } }
        public string prop_media_video_resolution { set { this.lblVideoResolution.Text = value; } }
        public string prop_media_audio_codec { set { this.lblAudioCodec.Text = value; } }
        public string prop_media_audio_rate { set { this.lblAudioRate.Text = value; } }

        
        public bool prop_isFullscreen { get { return this.isFullScreen;  } }
        public bool prop_isRecording { set { lblrecordIndication.Visible = value;  } }

        // quick tune variables *********************************************************************
        private static readonly Object list_lock = new Object();

        static int height = 255;    //makes things easier
        static int bandplan_height = 30;

        Bitmap bmp;
        static Bitmap bmp2;
        Pen greyPen = new Pen(Color.FromArgb(200, 123, 123, 123));
        Pen greyPen2 = new Pen(Color.FromArgb(200, 123, 123, 123));
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
        string TX_Text;  //dh3cs

        List<string> blocks = new List<string>();

        socket sock;
        signal sigs;

        int num_rxs_to_scan = 1;

        bool prevLocked = false;

        uint current_frequency = 0;
        uint current_sr = 0;
        bool current_enable_lnb_supply = false;
        bool current_enable_horiz_supply = false;
        bool current_rf_input = false; // true is A, false is B
       

        byte rxVolume = 100; // todo, save volume between sessions
        byte beforeMute = 0;

        // settings values
        string setting_snapshot_path = "";
        bool setting_enable_spectrum = true;
        byte setting_default_lnb_supply = 0;    // 0 - off, 1 - vert, 2 - horiz
        int setting_default_lo_value_1 = 0;
        int setting_default_lo_value_2 = 0;
        int setting_default_volume = 100;
        int setting_language = 0;
        bool setting_enable_chatform = false;
        bool setting_auto_connect = false;

        int setting_window_width = -1;
        int setting_window_height = -1;
        int setting_window_x = -1;
        int setting_window_y = -1;

        Font setting_chat_font;
        int setting_chat_width = 0;
        int setting_chat_height = 0;
        bool setting_disable_lna = false;

        bool isFullScreen = false;

        private wbchat chatForm;

        List<StoredFrequency> stored_frequencies = new List<StoredFrequency>();

        public void toggleFullScreen()
        {
            if (isFullScreen)
            {
                videoView1.MediaPlayer.SetMarqueeInt(VideoMarqueeOption.Enable, 0);

                this.FormBorderStyle = FormBorderStyle.Sizable;
                this.WindowState = FormWindowState.Normal;
                menuStrip1.Visible = true;
                splitContainer1.Panel1Collapsed = false;
                splitContainer1.Panel1.Enabled = true;

                if (setting_enable_spectrum)
                {
                    splitContainer2.Panel2Collapsed = false;
                    splitContainer2.Panel2.Enabled = true;
                }

                isFullScreen = false;
            }
            else
            {
                videoView1.MediaPlayer.SetMarqueeInt(VideoMarqueeOption.Enable, 1);

                this.FormBorderStyle = FormBorderStyle.None;
                this.WindowState = FormWindowState.Maximized;
                menuStrip1.Visible = false;

                // hide status
                splitContainer1.Panel1Collapsed = true;
                splitContainer1.Panel1.Enabled = false;

                // hide spectrum
                if (setting_enable_spectrum)
                {
                    splitContainer2.Panel2Collapsed = true;
                    splitContainer2.Panel2.Enabled = false;
                }
                isFullScreen = true;

                videoView1.MediaPlayer.EnableMouseInput = false;

            }
        }

        //         private delegate void updateRecordingStatusDelegate(Form1 gui, bool recording_status);

        public static void updateRecordingStatus(Form1 gui, bool recording_status)
        {
            if (gui.InvokeRequired)
            {
                updateRecordingStatusDelegate ulb = new updateRecordingStatusDelegate(updateRecordingStatus);
                gui.Invoke(ulb, new object[] { gui, recording_status });
            }
            else
            {
                gui.prop_isRecording = recording_status;
            }

        }


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
                gui.prop_null_packets_bar = Convert.ToInt32(new_status.NullPacketsPerc);

                
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
                double mer = Convert.ToDouble(new_status.mer) / 10;
                gui.prop_mer = mer.ToString() + " dB";
                gui.prop_lnagain = new_status.lna_gain.ToString();
                //gui.prop_power_i = new_status.power_i.ToString();
                //gui.prop_power_q = new_status.power_q.ToString();
                gui.prop_rf_input_level = new_status.input_power_level.ToString() + " dB";
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
                    gui.prop_null_packets_bar = 0;

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

                // vlc marquee on fullscreen
                if (gui.isFullScreen)
                {
                    string marquee = lookups.demod_state_lookup[new_status.demod_status] + " - " + gui.prop_db_margin + "(" + gui.prop_mer + ") - " + gui.prop_modcod + " - " + gui.prop_service_name;

                    MediaPlayer mediaPlayer = gui.prop_media_player;
                    mediaPlayer.SetMarqueeString(VideoMarqueeOption.Text, marquee);
                }
            }
        }
        public Form1()
        {
            //Thread.CurrentThread.CurrentUICulture = new CultureInfo("de-DE");
            load_settings();

            if (setting_language > 0)
            {
                switch(setting_language)
                {
                    case 1: Thread.CurrentThread.CurrentUICulture = new CultureInfo("de-DE");
                        break;
                    case 2:
                        Thread.CurrentThread.CurrentUICulture = new CultureInfo("ja-JP");
                        break;
                    case 3:
                        Thread.CurrentThread.CurrentUICulture = new CultureInfo("it-IT");
                        break;
                    case 4:
                        Thread.CurrentThread.CurrentUICulture = new CultureInfo("nl-NL");
                        break;
                    case 5:
                        Thread.CurrentThread.CurrentUICulture = new CultureInfo("pl-PL");
                        break;
                    case 6:
                        Thread.CurrentThread.CurrentUICulture = new CultureInfo("es-ES");
                        break;
                    case 7:
                        Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-EN");
                        break;
                    default:
                        break;
                }
                
            }

            InitializeComponent();

            greyPen.DashCap = System.Drawing.Drawing2D.DashCap.Round;
            greyPen.DashPattern = new float[] { 4.0F, 4.0F };

            greyPen2.DashCap = System.Drawing.Drawing2D.DashCap.Round;
            greyPen2.DashPattern = new float[] { 1F, 4.0F };
        }

        public void start_video()
        {
            Console.WriteLine("Main: Starting VLC");

            if (videoView1.MediaPlayer != null)
            {
                videoView1.MediaPlayer.Play(media);
            }
            

        }

        public void stop_video()
        {
            Console.WriteLine("Main: Stopping VLC");

            if (videoView1.MediaPlayer != null)
                videoView1.MediaPlayer.Stop();

            Console.WriteLine("Main: Stopping Recording");

            if (ts_recorder != null)
                ts_recorder.record = false;

            if (ts_udp != null)
                ts_udp.stream = false;
        }

        private void hardware_init()
        {
            ftdi_hw = new ftdi();

            // detect ftdi devices
            uint i2c_port = 99;
            uint ts_port = 99;
            string deviceName = "Unknown";

            byte err = ftdi_hw.ftdi_detect(ref i2c_port, ref ts_port, ref deviceName);


            if (i2c_port == 99 || ts_port == 99)    // not detected properly, revert to 0 and 1 and hope for the best
            {
                Console.WriteLine("Hardware not detected properly, reverting to 0,1");
                err = ftdi_hw.ftdi_init(0, 1);
            }
            else
            {
                Console.WriteLine("Trying detected ports:");
                Console.WriteLine("i2c port: " + i2c_port.ToString());
                Console.WriteLine("ts port: " + ts_port.ToString());
                err = ftdi_hw.ftdi_init(i2c_port, ts_port);
            }

            if (err != 0)
            {
                Console.WriteLine("Main: Error: FTDI Failed " + err.ToString());
                hardware_connected = false;
                return;
            }

            hardware_connected = true;

            this.Text = this.Text + " - " + deviceName;
        }

        private void MediaPlayer_EncounteredError(object sender, EventArgs e)
        {
            Console.WriteLine("VLC: Error: " + libVLC.LastLibVLCError);
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

            updateNimStatusGui(this, nim_status);

            if (prevLocked != locked)
            {
                Console.WriteLine("Lock State Change: " + prevLocked.ToString() + "->" + locked.ToString());

                if (nim_status.demod_status >= 2)
                {
                    //Console.WriteLine("Startng TS queue and VLC");
                    //nim_status.build_queue = true;
                    //ts_build_queue_flag = true;
                    start_video();
                }
                else
                {
                    //Console.WriteLine("Stopping TS queue and VLC");
                    //nim_status.build_queue = false;
                    //ts_build_queue_flag = false;
                    stop_video();
                }

                prevLocked = locked;
            }
            else
            {
                //nim_status.build_queue = ts_build_queue_flag;
            }


            // inform ts thread of whats happening
            //ts_status_queue.Enqueue(nim_status);


        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // save chat location settings
            if (setting_enable_chatform && chatForm != null)
            {
                Properties.Settings.Default.wbchat_height = chatForm.Size.Width;
                Properties.Settings.Default.wbchat_width = chatForm.Size.Height;
            }

            // save current volume as default volume
            Properties.Settings.Default.default_volume = trackVolume.Value;

            // save current windows properties
            Properties.Settings.Default.window_width = this.Width;
            Properties.Settings.Default.window_height = this.Height;
            Properties.Settings.Default.window_x = this.Left;
            Properties.Settings.Default.window_y = this.Top;


            Properties.Settings.Default.Save();


            stop_video();

            if (mediaInput != null)
                mediaInput.Dispose();
            if (media != null)
                media.Dispose();

            if (nim_thread_t != null)
                nim_thread_t.Abort();

            if (ts_recorder_t != null)
                ts_recorder_t.Abort();
            if (ts_udp_t != null)
                ts_udp_t.Abort();
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

            NimThread nim_thread = new NimThread(config_queue, ftdi_hw, status_callback, setting_disable_lna);

            nim_thread_t = new Thread(nim_thread.worker_thread);

            NimConfig initialConfig = new NimConfig();
            initialConfig.frequency = 741525;
            initialConfig.symbol_rate = 1500;

            current_frequency = initialConfig.frequency;
            current_sr = initialConfig.symbol_rate;
            initialConfig.polarization_supply = current_enable_lnb_supply;
            initialConfig.polarization_supply_horizontal = current_enable_horiz_supply;

            // we need to make sure we have a config queued before starting the thread
            config_queue.Enqueue(initialConfig);

            nim_thread_t.Start();

            Console.WriteLine("Main: Starting TS Thread");

            // TS thread

            TSThread ts_thread = new TSThread(ftdi_hw, ts_data_queue, nim_thread);
            ts_thread_t = new Thread(ts_thread.worker_thread);
            ts_thread_t.Start();

            // TS Parser Thread
            ts_thread.RegisterTSConsumer(ts_parser_data_queue);

            TSDataCallback ts_data_callback = new TSDataCallback(parse_ts_data_callback);
            TSParserThread ts_parser_thread = new TSParserThread(ts_data_callback, ts_parser_data_queue);
            ts_parser_t = new Thread(ts_parser_thread.worker_thread);
            ts_parser_t.Start();

            // TS recorder Thread
            ts_recorder = new TSRecorderThread(ts_thread, setting_snapshot_path);
            ts_recorder.onRecordStatusChange += Ts_recorder_onRecordStatusChange;
            ts_recorder_t = new Thread(ts_recorder.worker_thread);
            ts_recorder_t.Start();

            // TS udp thread
            ts_udp = new TSUDPThread(ts_thread);
            ts_udp_t = new Thread(ts_udp.worker_thread);
            ts_udp_t.Start();

            //libVLC.Log += LibVLC_Log;

            videoView1.MediaPlayer = new MediaPlayer(libVLC);
            videoView1.MediaPlayer.Stopped += MediaPlayer_Stopped;
            videoView1.MediaPlayer.Playing += MediaPlayer_Playing;
            videoView1.MediaPlayer.EncounteredError += MediaPlayer_EncounteredError;
            videoView1.MediaPlayer.Vout += MediaPlayer_Vout;            

            videoView1.MediaPlayer.EnableMouseInput = false;
            videoView1.MediaPlayer.EnableKeyInput = false;

            videoView1.MediaPlayer.SetMarqueeInt(VideoMarqueeOption.Size, 20);
            videoView1.MediaPlayer.SetMarqueeInt(VideoMarqueeOption.X, 10);
            videoView1.MediaPlayer.SetMarqueeInt(VideoMarqueeOption.Y, 10);

            videoView1.MouseWheel += VideoView1_MouseWheel;



            //string udp_destination = "127.0.0.1:8090";
            mediaInput = new TSStreamMediaInput(ts_data_queue);
            //media = new Media(libVLC, mediaInput, ":sout-keep", ":sout=#duplicate{dst=display, dst=std{access=udp, mux=ts, dst=" + udp_destination + "}}" );
            media = new Media(libVLC, mediaInput /*,  ":sout-keep", ":sout=#duplicate{dst=display, dst=std{access=udp, mux=ts, dst=" + udp_destination + "}}" */);

            MediaConfiguration mediaConfig = new MediaConfiguration();            
            mediaConfig.EnableHardwareDecoding = false;
            media.AddOption(mediaConfig);
            /*
            media.AddOption(":sout-keep");
            media.AddOption(":sout=#duplicate{dst=display, dst=std{access=udp, mux=ts, dst=" + udp_destination + "}}");
            media.AddOption(":no-overlay");
            //media.AddOption(":sout=#std{access=udp, mux=ts, dst=" + udp_destination + "}");
            */



            // temporary to prevent multiple connection attempts
            // todo: deal with this properly
            lblConnected.Text = "Connected";
            lblConnected.ForeColor = Color.Green;

            menuConnect.Enabled = false;
        }

        private void VideoView1_MouseWheel(object sender, MouseEventArgs e)
        {
            byte volumeDelta = 1;
            if (e.Delta < 0 && rxVolume >= (0 + volumeDelta))
            {
                rxVolume -= volumeDelta;
                trackVolume.Value = rxVolume;
                return;
            }

            if (e.Delta > 0 && rxVolume <= (200 - volumeDelta))
            {
                rxVolume += volumeDelta;
                trackVolume.Value = rxVolume;
                return;
            }

        }

        private void Ts_recorder_onRecordStatusChange(object sender, bool e)
        {
            updateRecordingStatus(this, e);
        }

        private void LibVLC_Log(object sender, LogEventArgs e)
        {
            debug("VLC Log: " + e.FormattedLog + "," + e.Level);
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
            videoView1.MediaPlayer.Volume = rxVolume;

            if (checkRecordAll.Checked)
            {
                if (ts_recorder != null)
                    ts_recorder.record = true;  // recording will automatically stop when lock is lost
            }

            if (checkUDPEnable.Checked)
            {
                if (ts_udp != null)
                {
                    ts_udp.stream = true;
                }
                
            }

        }

        private void btnFrequencyChange_Click(object sender, EventArgs e)
        {
            UInt32 freq = Convert.ToUInt32(txtFreq.Text);
            UInt32 lo = Convert.ToUInt32(txtLO.Text);
            UInt32 sr = Convert.ToUInt32(txtSR.Text);

            change_frequency(freq, lo, sr);
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

            int i = 1;

            for (i = 1; i <= 4; i++)
            {
                int y = spectrum_h - ((i * (spectrum_h / 4)) - (spectrum_h / 6));
                tmp.DrawLine(greyPen, 10, y, spectrum_w - 10, y);
            }


            PointF[] points = new PointF[fft_data.Length - 2];


            for (i = 1; i < fft_data.Length - 3; i++)     //ignore padding?
            {
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
            tmp.DrawString(TX_Text, new Font("Tahoma", 15), Brushes.Red, new PointF(70, spectrum.Height - 50));  //dh3cs


            //drawspectrum_signals(sigs.detect_signals(fft_data));
            sigs.detect_signals(fft_data);

            // draw over power
            foreach (var sig in sigs.signalsData)
            {
                if (sig.overpower)
                {
                    tmp.FillRectangles(overpowerBrush, new RectangleF[] { new System.Drawing.Rectangle(Convert.ToInt16(sig.fft_centre * spectrum_wScale) - (Convert.ToInt16((sig.fft_stop - sig.fft_start) * spectrum_wScale)  / 2), 1, Convert.ToInt16((sig.fft_stop - sig.fft_start) * spectrum_wScale), (255) - 4) });
                }
            }

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

                string tx_freq = get_bandplan_TX_freq(X, Y);
                debug("TX-Freq: " + tx_freq + " MHz");
                // dh3cs
                if (!string.IsNullOrEmpty(tx_freq))
                {
                    //Clipboard.SetText((Convert.ToDecimal(tx_freq) * 1000).ToString());    //DATV Express in Hz
                    Clipboard.SetText(tx_freq);                                             //DATV-Easy in MHz
                    TX_Text = " TX: " + tx_freq;
                }

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
            get_bandplan_TX_freq(e.X, e.Y);  // dh3cs
        }

        // moved to separate function, to use by right click in spectrum,  dh3cs 
        private string get_bandplan_TX_freq(int x, int y)  // returns TX-Freq in MHz from the rectangle in Bandplan
        {
            int n = 0;
            string tx_freq_MHz = "";
            if (x > (spectrum.Height - bandplan_height))
            {
                if (channels != null)
                {
                    foreach (Rectangle ch in channels)
                    {
                        if (x >= ch.Location.X & x <= ch.Location.X + ch.Width)
                        {
                            if (y - (spectrum.Height - bandplan_height) >= ch.Location.Y - (ch.Height / 2) + 3 & y - (spectrum.Height - bandplan_height) <= ch.Location.Y + (ch.Height / 2) + 3)
                            {
                                tx_freq_MHz = indexedbandplan[n].Element("s-freq").Value;
                                InfoText = " Dn: " + indexedbandplan[n].Element("x-freq").Value + "  SR: " + indexedbandplan[n].Element("name").Value + Environment.NewLine
                                    + " Up: " + tx_freq_MHz;
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
            return tx_freq_MHz;
        }
        
        private void load_settings()
        {
            // warning: don't use the debug function in this function as it gets called before components are initialized

            Console.WriteLine("System Culture Setting: " + CultureInfo.CurrentCulture.Name);

            Properties.Settings.Default.Reload();

            setting_default_lnb_supply = Properties.Settings.Default.default_lnb_supply;
            setting_default_lo_value_1 = Properties.Settings.Default.tuner1_default_lo;
            setting_default_lo_value_2 = Properties.Settings.Default.tuner2_default_lo;
            setting_enable_spectrum = Properties.Settings.Default.enable_qo100_spectrum;
            setting_snapshot_path = Properties.Settings.Default.media_snapshot_path;
            setting_language = Properties.Settings.Default.language;
            setting_default_volume = Properties.Settings.Default.default_volume;

            setting_chat_font = Properties.Settings.Default.wbchat_font;
            setting_chat_width = Properties.Settings.Default.wbchat_width;
            setting_chat_height = Properties.Settings.Default.wbchat_height;
            setting_enable_chatform = Properties.Settings.Default.wbchat_enable;

            setting_window_width = Properties.Settings.Default.window_width;
            setting_window_height = Properties.Settings.Default.window_height;
            setting_window_x = Properties.Settings.Default.window_x;
            setting_window_y = Properties.Settings.Default.window_y;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            debug("System Culture Setting: " + CultureInfo.CurrentCulture.Name);
            debug("Settings: Restore Last Volume: " + setting_default_volume.ToString() + "%");
            trackVolume.Value = setting_default_volume;
            debug("Settings: Default LO A: " + setting_default_lo_value_1.ToString());
            txtLO.Text = setting_default_lo_value_1.ToString();
            debug("Settings: Default LO B: " + setting_default_lo_value_2.ToString());

            if (setting_snapshot_path.Length == 0)
                setting_snapshot_path = AppDomain.CurrentDomain.BaseDirectory;

            debug("Settings: Snapshot Path: " + setting_snapshot_path);

            switch (setting_default_lnb_supply)
            {
                case 0:
                    current_enable_lnb_supply = false;
                    current_enable_horiz_supply = false;
                    radioLnbSupplyOff.Checked = true;
                    break;
                case 1:
                    current_enable_lnb_supply = true;
                    current_enable_horiz_supply = false;
                    radioLnbSupplyVert.Checked = true;
                    break;
                case 2:
                    current_enable_lnb_supply = true;
                    current_enable_horiz_supply = true;
                    radioLnbSupplyHoriz.Checked = true;
                    break;
            }

            debug("Settings: Enable LNB Supply: " + current_enable_lnb_supply.ToString());

            if (current_enable_lnb_supply)
            {
                debug("Settings: Enable Vert Supply: " + (!current_enable_horiz_supply).ToString());
                debug("Settings: Enable Horiz Supply: " + current_enable_horiz_supply.ToString());
            }

            string[] args = Environment.GetCommandLineArgs();

            bool first = true;
            
            foreach (string arg in args)
            {
                if (first)
                {
                    first = false;
                    continue;
                }

                Console.WriteLine(arg);

                if (arg == "DISABLELNA")
                {
                    Console.WriteLine("Disabling LNA Configure due to command line");
                    setting_disable_lna = true;
                }

                if (arg == "DISABLEQO100")
                {
                    Console.WriteLine("Disabling QO-100 Features due to command line");
                    setting_enable_chatform = false;
                    setting_enable_spectrum = false;
                }

                if (arg == "AUTOCONNECT")
                {
                    Console.WriteLine("Auto Connect Enabled due to command line");
                    setting_auto_connect = true;
                }
            }

            if (setting_enable_chatform)
            {
                qO100WidebandChatToolStripMenuItem.Visible = true;
                chatForm = new wbchat();

                if (setting_chat_width > -1 && setting_chat_height > -1)
                {
                    chatForm.Size = new Size(setting_chat_height, setting_chat_width);

                    if (setting_chat_font != null)
                    {
                        chatForm.lbChat.Font = setting_chat_font;
                        chatForm.lbUsers.Font = setting_chat_font;
                        chatForm.txtMessage.Font = setting_chat_font;
                    }
                }
            }
            else
            {
                qO100WidebandChatToolStripMenuItem.Visible = false;
            }


            if (setting_enable_spectrum)
            {
                debug("Settings: QO-100 Spectrum Enabled");

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
            else
            {
                debug("Settings: QO-100 Spectrum Disabled");

                splitContainer2.Panel2Collapsed = true;
                splitContainer2.Panel2.Enabled = false;
                websocketTimer.Enabled = false;
            }

            //setting_window_width = Properties.Settings.Default.window_width;
            //setting_window_height = Properties.Settings.Default.window_height;
            //setting_window_x = Properties.Settings.Default.window_x;
            //setting_window_y = Properties.Settings.Default.window_y;

            if (setting_window_height > -1 && setting_window_width > -1)
            {
                this.Height = setting_window_height;
                this.Width = setting_window_width;
            }

            if (setting_window_x > -1 && setting_window_y > -1)
            {
                this.Left = setting_window_x;
                this.Top = setting_window_y;
            }


            if (setting_auto_connect)
            {
                btnConnectTuner_Click(this, e);
            }


            load_stored_frequencies();
            rebuild_stored_frequencies();
        }

        private void rebuild_stored_frequencies()
        {
            storedFrequenciesToolStripMenuItem.DropDownItems.Clear();

            if (stored_frequencies.Count == 0)
            {
                storedFrequenciesToolStripMenuItem.Visible = false;
                return;
            }

            storedFrequenciesToolStripMenuItem.Visible = true;

            for (int c = 0; c < stored_frequencies.Count; c++)
            {
                ToolStripMenuItem sf_menu = new ToolStripMenuItem(stored_frequencies[c].Name + " (" + stored_frequencies[c].Frequency + ")");
                sf_menu.Tag = c;
                sf_menu.Click += Sf_menu_Click;

                storedFrequenciesToolStripMenuItem.DropDownItems.Add(sf_menu);
            }
        }

        private void Sf_menu_Click(object sender, EventArgs e)
        {
            int tag = Convert.ToInt32(((ToolStripMenuItem)(sender)).Tag);
            Console.WriteLine(tag.ToString());

            if (tag < stored_frequencies.Count)
            {
                tune_stored_frequency(stored_frequencies[tag]);
            }
        }

        void tune_stored_frequency(StoredFrequency sf)
        {
            NimConfig newConfig = new NimConfig();

            newConfig.frequency = sf.Frequency - sf.Offset;            
            newConfig.symbol_rate = sf.SymbolRate;
            newConfig.polarization_supply = current_enable_lnb_supply;
            newConfig.polarization_supply_horizontal = current_enable_horiz_supply;

            if (sf.RFInput == 0)
                newConfig.rf_input_B = false;
            else
                newConfig.rf_input_B = true;

            if (newConfig.frequency < 144000 || newConfig.frequency > 2450000)
            {
                debug("Error: Invalid Frequency: " + newConfig.frequency);
                lblFreqError.Text = "Invalid:" + newConfig.frequency.ToString();
                return;
            }
            else
            {
                lblFreqError.Text = "";
            }

            debug("Main: New Config: " + newConfig.ToString());

            current_frequency = newConfig.frequency;
            current_sr = newConfig.symbol_rate;
            current_rf_input = newConfig.rf_input_B;

            config_queue.Enqueue(newConfig);
        }

        void change_frequency(UInt32 freq, UInt32 sr, bool lnb_supply, bool polarization_supply_horizontal, bool rf_input_B)
        {
            NimConfig newConfig = new NimConfig();

            newConfig.frequency = freq;
            newConfig.symbol_rate = sr;
            newConfig.polarization_supply = lnb_supply;
            newConfig.polarization_supply_horizontal = polarization_supply_horizontal;
            newConfig.rf_input_B = rf_input_B;

            if (newConfig.frequency < 144000 || newConfig.frequency > 2450000)
            {
                debug("Error: Invalid Frequency: " + newConfig.frequency);
                lblFreqError.Text = "Invalid:" + newConfig.frequency.ToString();
                return;
            }
            else
            {
                lblFreqError.Text = "";
            }

            debug("Main: New Config: " + newConfig.ToString());

            current_frequency = newConfig.frequency;
            current_sr = sr;
            current_enable_lnb_supply = lnb_supply;
            current_enable_horiz_supply = polarization_supply_horizontal;
            current_rf_input = rf_input_B;

            config_queue.Enqueue(newConfig);
        }

        void change_frequency(UInt32 freq, UInt32 lo, UInt32 sr)
        {
            change_frequency(freq - lo, sr, current_enable_lnb_supply, current_enable_horiz_supply, current_rf_input);
        }

        void change_lnb_supply(bool enable_supply, bool horiz_supply)
        {
            change_frequency(current_frequency, current_sr, current_enable_lnb_supply, current_enable_horiz_supply, current_rf_input);
        }

        void change_rf_input(bool rf_input_b)
        {
            NimConfig newConfig = new NimConfig();

            newConfig.frequency = current_frequency;
            newConfig.symbol_rate = current_sr;
            newConfig.polarization_supply = current_enable_lnb_supply;
            newConfig.polarization_supply_horizontal = current_enable_horiz_supply;
            newConfig.rf_input_B = rf_input_b;

            current_rf_input = rf_input_b;

            debug("Main: New Config: " + newConfig.ToString());

            config_queue.Enqueue(newConfig);
        }

        private void load_stored_frequencies()
        {
            string frequency_file = AppDomain.CurrentDomain.BaseDirectory + "\\frequencies.json";
            string json = "";

            try
            {
                json = File.ReadAllText(frequency_file);
                stored_frequencies = JsonConvert.DeserializeObject<List<StoredFrequency>>(json);
            }
            catch(Exception Ex)
            {
                Console.WriteLine("Error reading frequencies.json - file missing, or wrong format :" + Ex.Message);
            }
        }

        private void save_stored_frequencies()
        {
            string json = JsonConvert.SerializeObject(stored_frequencies, Formatting.Indented);

            string frequency_file = AppDomain.CurrentDomain.BaseDirectory + "\\frequencies.json";

            try
            {
                File.WriteAllText(frequency_file, json);
            }
            catch(Exception Ex)
            {
                Console.WriteLine("Error writing frequencies file: " + Ex.Message);
            }
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

                if (bandplan != null)
                {
                    drawspectrum_bandplan();
                }
            }
            catch (Exception Ex)
            {

            }
        }

        private void btnMute_Click(object sender, EventArgs e)
        {
            if (rxVolume == 0)
            {
                rxVolume = beforeMute;
            }
            else
            {
                beforeMute = rxVolume;
                rxVolume = 0;
            }

            trackVolume.Value = rxVolume;
        }

        private void trackVolume_ValueChanged(object sender, EventArgs e)
        {
            rxVolume = Convert.ToByte(trackVolume.Value);

            if (videoView1.MediaPlayer != null)
            {
                videoView1.MediaPlayer.Volume = rxVolume;
            }

            lblVolume.Text = rxVolume.ToString() + " %";
        }

        private void TakeSnapshot()
        {

            if (videoView1.MediaPlayer == null)
                return;

            // get path
            string path = setting_snapshot_path;

            string filename = DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss") + ".png";

            if (lblServiceName.Text.Length > 0)
                filename = lblServiceName.Text.ToString() + "_" + filename;

            // remove any possible spaces
            filename = filename.Replace(" ", "");

            videoView1.MediaPlayer.TakeSnapshot(0, path + filename, 0, 0);
        }

        private void btnSnapshot_Click(object sender, EventArgs e)
        {
            TakeSnapshot();
        }

        private void radioLnbSupply_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked)  // all the radio buttons will fire this event, only run once for the checked event
            {
                stop_video();

                if (radioLnbSupplyOff.Checked)
                {
                    change_lnb_supply(false, false);
                    return;
                }

                if (radioLnbSupplyHoriz.Checked)
                {
                    change_lnb_supply(true, true);
                    return;
                }

                if (radioLnbSupplyVert.Checked)
                {
                    change_lnb_supply(true, false);
                    return;
                }
            }

        }

        private void openTunerWebsiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.zr6tg.co.za/open-tuner/");
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://batc.org.uk/");
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            settingsForm settings_form = new settingsForm();

            settings_form.txtDefaultLO.Text = setting_default_lo_value_1.ToString();
            settings_form.txtDefaultLO2.Text = setting_default_lo_value_2.ToString();
            settings_form.comboDefaultLNB.SelectedIndex = setting_default_lnb_supply;
            settings_form.txtSnapshotPath.Text = setting_snapshot_path;
            settings_form.checkEnableSpectrum.Checked = setting_enable_spectrum;
            settings_form.checkEnableChat.Checked = setting_enable_chatform;
            settings_form.comboLanguage.SelectedIndex = setting_language;


            if (setting_enable_chatform && chatForm != null)
            {
                settings_form.currentChatFont = chatForm.lbChat.Font;
            }
            else
            {
                if (setting_chat_font != null)
                    settings_form.currentChatFont = setting_chat_font;
                else
                    settings_form.currentChatFont = label8.Font; // just making sure there is somekind of font to start off with
            }


            if ( settings_form.ShowDialog() == DialogResult.OK )
            {
                Properties.Settings.Default.wbchat_font = settings_form.currentChatFont;

                Properties.Settings.Default.default_lnb_supply = Convert.ToByte(settings_form.comboDefaultLNB.SelectedIndex);
                Properties.Settings.Default.tuner1_default_lo = Convert.ToInt32(settings_form.txtDefaultLO.Text);
                Properties.Settings.Default.tuner2_default_lo = Convert.ToInt32(settings_form.txtDefaultLO2.Text);
                Properties.Settings.Default.enable_qo100_spectrum = settings_form.checkEnableSpectrum.Checked;
                Properties.Settings.Default.wbchat_enable = settings_form.checkEnableChat.Checked;
                Properties.Settings.Default.media_snapshot_path = settings_form.txtSnapshotPath.Text;

                setting_language = settings_form.comboLanguage.SelectedIndex;

                Properties.Settings.Default.language = setting_language;
                Properties.Settings.Default.Save();
                
                load_settings();
            }
        }

        private void lblConnected_Click(object sender, EventArgs e)
        {
            if (!hardware_connected)
                btnConnectTuner_Click(this, e);
        }

        private void radioRFInput_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked)  // all the radio buttons will fire this event, only run once for the checked event
            {
                stop_video();

                if (radioRFInputA.Checked)
                {
                    change_rf_input(false);
                    txtLO.Text = setting_default_lo_value_1.ToString();
                }
                else
                {
                    change_rf_input(true);
                    txtLO.Text = setting_default_lo_value_2.ToString();
                }
            }
        }

        private void menuFullScreen_Click(object sender, EventArgs e)
        {
            toggleFullScreen();
        }

        private void qO100WidebandChatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            chatForm.Show();
        }

        private void lblServiceName_TextChanged(object sender, EventArgs e)
        {
            if (setting_enable_spectrum)
            {
                // we have decoded a callsign
                string callsign = lblServiceName.Text;
                int offset = 0;

                Int32.TryParse(txtLO.Text, out offset);

                if (callsign.Length > 0)
                {
                    double freq = current_frequency + offset;
                    freq = freq / 1000;
                    float sr = current_sr;

                    debug("New Callsign: " + callsign + "," + freq.ToString() + "," + sr.ToString());
                    sigs.updateCurrentSignal(callsign, freq, sr);

                }
            }
        }

        private void radioSpectrumTune_CheckedChanged(object sender, EventArgs e)
        {
            if ( ((RadioButton)sender).Checked )
            {
                if (radioSpectrumTuneManual.Checked)
                {
                    SpectrumTuneTimer.Enabled = false;
                }
                else
                {
                    SpectrumTuneTimer.Enabled = true;
                }
            }
        }

        private void SpectrumTuneTimer_Tick(object sender, EventArgs e)
        {
            int mode = 0;
            float spectrum_w = spectrum.Width;
            float spectrum_wScale = spectrum_w / 922;

            if (radioSpectrumTuneAutoTimed.Checked)
            {
                mode = 2;
            }
            else
            {
                mode = 1;
            }

            //float time = Convert.ToSingle(0.5, CultureInfo.InvariantCulture);
            ushort autotuneWait = 30;

            Tuple<signal.Sig, int> ret = sigs.tune(mode, Convert.ToInt16(autotuneWait), 0);
            if (ret.Item1.frequency > 0)      //above 0 is a change in signal
            {
                System.Threading.Thread.Sleep(100);
                selectSignal(Convert.ToInt32(ret.Item1.fft_centre * spectrum_wScale));
                sigs.set_tuned(ret.Item1, 0);
                rx_blocks[0] = Convert.ToInt16(ret.Item1.fft_centre);
                rx_blocks[1] = Convert.ToInt16(ret.Item1.fft_stop - ret.Item1.fft_start);
            }

        }

        private void btnRecord_Click(object sender, EventArgs e)
        {
            if (ts_recorder != null)
            {
                if (ts_recorder.record)
                    ts_recorder.record = false;
                else
                    ts_recorder.record = true;
            }
        }

        private void checkUDPEnable_CheckedChanged(object sender, EventArgs e)
        {
            if (ts_udp != null)
            {
                if (checkUDPEnable.Checked)
                {
                    ts_udp.stream = true;
                }
                else
                {
                    ts_udp.stream = false;
                }
            }
        }

        private void videoView1_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                toggleFullScreen();
            }
            catch (Exception Ex)
            {

            }
        }

        private void toggleFullscreenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                toggleFullScreen();
            }
            catch (Exception Ex)
            {

            }
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            if (videoView1.MediaPlayer != null)
                videoView1.MediaPlayer.AspectRatio = "4:3";
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            if (videoView1.MediaPlayer != null)
                videoView1.MediaPlayer.AspectRatio = "16:9";
        }

        private void manageStoredFrequenciesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frequencyManagerForm freqForm = new frequencyManagerForm(stored_frequencies);
            freqForm.ShowDialog();

            save_stored_frequencies();
            rebuild_stored_frequencies();
        }
    }


}
