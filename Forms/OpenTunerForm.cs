using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using LibVLCSharp.Shared;
using LibVLCSharp.WinForms;
using Newtonsoft.Json;
using opentuner.Classes;
using opentuner.Hardware;
using opentuner.MediaPlayers;
using opentuner.Transport;

namespace opentuner.Forms
{
    public partial class OpenTunerForm : Form
    {
        OTMediaPlayer media_player_1;
        OTMediaPlayer media_player_2;

        MinitiounerSource mt = new MinitiounerSource();

        private Color _darkTheme = Color.FromArgb(63, 70, 76);
        private Color _lightTheme = Color.LightGray;
        private List<ToolStripMenuItem> toolSripItems = null;

        private delegate void updateNimStatusGuiDelegate(OpenTunerForm gui, TunerStatus new_status);

        private delegate void updateTSStatusGuiDelegate(int device, OpenTunerForm gui, TSStatus new_status);

        private delegate void updateMediaStatusGuiDelegate(int tuner, OpenTunerForm gui, MediaStatus new_status);

        private delegate void UpdateLBDelegate(ListBox LB, Object obj);

        private delegate void updateRecordingStatusDelegate(OpenTunerForm gui, bool recording_status, string id);

        Thread ts_parser_t = null;
        Thread ts_parser_2_t = null;
        Thread ts_recorder_1_t = null;
        Thread ts_recorder_2_t = null;
        Thread ts_udp_t1 = null;
        Thread ts_udp_t2 = null;

        TSRecorderThread ts_recorder1;
        TSRecorderThread ts_recorder2;
        TSUDPThread ts_udp1;
        TSUDPThread ts_udp2;

        // test
        //BBFrameUDP bbframe_udp1;
        //Thread bbframe_udp_t1 = null;

        // form properties
        //public MediaPlayer prop_media_player { get { return this.videoView1.MediaPlayer; } }

        // nim status properties

        // tuner 1 properties
        public string prop_demodstate
        {
            set { lblDemoState.Text = value; }
        }

        public string prop_mer
        {
            set { lblMer.Text = value; }
            get { return lblMer.Text; }
        }

        public string prop_lnagain
        {
            set { lblLnaGain.Text = value; }
        }

        public string prop_rf_input_level
        {
            set { lblRFInputLevel.Text = value; }
        }

        public string prop_symbol_rate
        {
            set { lblSR.Text = value; }
        }

        public string prop_modcod
        {
            set { lblModcod.Text = value; }
            get { return lblModcod.Text; }
        }

        public string prop_lpdc_errors
        {
            set { lblLPDCError.Text = value; }
        }

        public string prop_ber
        {
            set { lblBer.Text = value; }
        }

        public string prop_freq_carrier_offset
        {
            set { lblFreqCar.Text = value; }
        }

        public string prop_db_margin
        {
            set { lbldbMargin.Text = value; }
            get { return lbldbMargin.Text; }
        }

        public string prop_req_freq
        {
            set { lblReqFreq.Text = value; }
        }

        public int prop_rf_input
        {
            set { lblRfInput.Text = (value == 1 ? "A" : "B"); }
        }

        // tuner 2 properties
        public string prop_demodstate2
        {
            set { lblDemoState2.Text = value; }
        }

        public string prop_mer2
        {
            set { lblMer2.Text = value; }
            get { return lblMer2.Text; }
        }

        public string prop_lnagain2
        {
            set { lblLnaGain2.Text = value; }
        }

        public string prop_rf_input_level2
        {
            set { lblRFInputLevel2.Text = value; }
        }

        public string prop_symbol_rate2
        {
            set { lblSR2.Text = value; }
        }

        public string prop_modcod2
        {
            set { lblModcod2.Text = value; }
            get { return lblModcod2.Text; }
        }

        public string prop_ber2
        {
            set { lblBer2.Text = value; }
        }

        public string prop_freq_carrier_offset2
        {
            set { lblFreqCar2.Text = value; }
        }

        public string prop_db_margin2
        {
            set { lbldbMargin2.Text = value; }
            get { return lbldbMargin2.Text; }
        }

        public string prop_req_freq2
        {
            set { lblReqFreq2.Text = value; }
        }

        public int prop_rf_input2
        {
            set { lblRfInput2.Text = (value == 1 ? "A" : "B"); }
        }


        // tuner 1 - ts status properties

        public string prop_stream_format1
        {
            set { lblStreamFormat1.Text = value; }
        }

        public string prop_service_name
        {
            set { lblServiceName.Text = value; }
            get { return lblServiceName.Text; }
        }

        public string prop_service_provider_name
        {
            set { lblServiceProvider.Text = value; }
            get { return lblServiceProvider.Text; }
        }

        public string prop_null_packets
        {
            set { lblNullPackets.Text = value; }
        }

        public int prop_null_packets_bar
        {
            set { nullPacketsBar.Value = value; }
        }

        // tuner 2 - ts status properties
        public string prop_stream_format2
        {
            set { lblStreamFormat2.Text = value; }
        }

        public string prop_service_name2
        {
            set { lblServiceName2.Text = value; }
            get { return lblServiceName2.Text; }
        }

        public string prop_service_provider_name2
        {
            set { lblServiceProvider2.Text = value; }
            get { return lblServiceProvider2.Text; }
        }

        public string prop_null_packets2
        {
            set { lblNullPackets2.Text = value; }
        }

        public int prop_null_packets_bar2
        {
            set { nullPacketsBar2.Value = value; }
        }


        // media status properties
        public string prop_media_video_codec
        {
            set { lblVideoCodec.Text = value; }
        }

        public string prop_media_video_resolution
        {
            set { lblVideoResolution.Text = value; }
        }

        public string prop_media_audio_codec
        {
            set { lblAudioCodec.Text = value; }
        }

        public string prop_media_audio_rate
        {
            set { lblAudioRate.Text = value; }
        }

        // media status properties
        public string prop_media_video_codec2
        {
            set { lblVideoCodec2.Text = value; }
        }

        public string prop_media_video_resolution2
        {
            set { lblVideoResolution2.Text = value; }
        }

        public string prop_media_audio_codec2
        {
            set { lblAudioCodec2.Text = value; }
        }

        public string prop_media_audio_rate2
        {
            set { lblAudioRate2.Text = value; }
        }

        public bool prop_isRecording1
        {
            set { lblrecordIndication1.Visible = value; }
        }

        public bool prop_isRecording2
        {
            set { lblRecordIndication2.Visible = value; }
        }

        // quick tune variables *********************************************************************
        private static readonly Object list_lock = new Object();

        static int height = 255; //makes things easier
        static int bandplan_height = 30;

        Bitmap bmp;
        static Bitmap bmp2;
        Pen greyPen = new Pen(Color.FromArgb(200, 123, 123, 123));
        Pen greyPen2 = new Pen(Color.FromArgb(200, 123, 123, 123));
        SolidBrush shadowBrush = new SolidBrush(Color.FromArgb(128, Color.Gray));
        SolidBrush bandplanBrush = new SolidBrush(Color.FromArgb(180, 250, 250, 255));
        SolidBrush overpowerBrush = new SolidBrush(Color.FromArgb(128, Color.Red));

        SolidBrush tuner1Brush = new SolidBrush(Color.FromArgb(50, Color.Blue));
        SolidBrush tuner2Brush = new SolidBrush(Color.FromArgb(50, Color.Green));

        Graphics tmp;
        Graphics tmp2;

        int[,] rx_blocks = new int[2, 3];

        double start_freq = 10490.5f;

        XElement bandplan;
        Rectangle[] channels;
        IList<XElement> indexedbandplan;
        string InfoText;
        string TX_Text; //dh3cs

        List<string> blocks = new List<string>();

        socket sock;
        signal sigs;

        int num_rxs_to_scan = 1;

        bool T1P2_prevLocked = false;
        bool T2P1_prevLocked = false;

        byte rxVolume = 100;
        byte rxVolume2 = 100;
        byte beforeMute = 0;
        byte beforeMute2 = 0;

        // settings values
        string setting_snapshot_path = "";
        bool setting_enable_spectrum = true;
        byte setting_default_lnb_supply = 0; // 0 - off, 1 - vert, 2 - horiz
        int setting_default_lo_value_1 = 0;
        int setting_default_lo_value_2 = 0;
        int setting_default_volume = 100;
        int setting_default_volume2 = 100;
        int setting_language = 0;
        bool setting_enable_chatform = false;
        bool setting_auto_connect = false;
        bool setting_enable_darkmode = false;

        int setting_mediaplayer_1 = 0;
        int setting_mediaplayer_2 = 1;

        int setting_window_width = -1;
        int setting_window_height = -1;
        int setting_window_x = -1;
        int setting_window_y = -1;
        int setting_main_splitter_position = 436;

        //Font setting_chat_font;
        int setting_chat_font_size = 12;
        int setting_chat_width = 0;
        int setting_chat_height = 0;
        bool setting_disable_lna = false;

        string setting_udp_address1 = "127.0.0.1";
        int setting_udp_port1 = 9080;
        string setting_udp_address2 = "127.0.0.1";
        int setting_udp_port2 = 9081;

        bool setting_windowed_mediaPlayer1 = false;
        bool setting_windowed_mediaPlayer2 = false;

        string setting_tuner1_startfreq = "Default";
        string setting_tuner2_startfreq = "Default";

        string setting_sigreport_template = "SigReport: {SN}/{SP} - {DBM} - ({MER}) - {SR} - {FREQ}";

        // custom forms
        private WideBandChatForm chatForm;
        private TunerControlForm tuner1ControlForm;
        private TunerControlForm tuner2ControlForm;
        private VideoViewForm mediaPlayer1Window;
        private VideoViewForm mediaPlayer2Window;

        List<StoredFrequency> stored_frequencies = new List<StoredFrequency>();
        List<ExternalTool> external_tools = new List<ExternalTool>();

        // datv easy - commands
        UdpClient tx_client = new UdpClient();
        System.Net.IPEndPoint tx_end_point;

        bool setting_autodetect = true;
        string[] manual_serial_devices;


        private async void SoftBlink(Control ctrl, Color c1, Color c2, short CycleTime_ms, bool BkClr)
        {
            var sw = new Stopwatch();
            sw.Start();
            short halfCycle = (short)Math.Round(CycleTime_ms * 0.5);
            while (true)
            {
                await Task.Delay(1);
                var n = sw.ElapsedMilliseconds % CycleTime_ms;
                var per = (double)Math.Abs(n - halfCycle) / halfCycle;
                var red = (short)Math.Round((c2.R - c1.R) * per) + c1.R;
                var grn = (short)Math.Round((c2.G - c1.G) * per) + c1.G;
                var blw = (short)Math.Round((c2.B - c1.B) * per) + c1.B;
                var clr = Color.FromArgb(red, grn, blw);
                if (BkClr) ctrl.BackColor = clr;
                else ctrl.ForeColor = clr;
            }
        }

        public static void updateRecordingStatus(OpenTunerForm gui, bool recording_status, string id)
        {
            if (gui == null)
                return;

            if (gui.InvokeRequired)
            {
                updateRecordingStatusDelegate ulb = new updateRecordingStatusDelegate(updateRecordingStatus);

                if (gui != null)
                    gui.Invoke(ulb, new object[] { gui, recording_status, id });
            }
            else
            {
                if (id == "t1")
                    gui.prop_isRecording1 = recording_status;
                else
                    gui.prop_isRecording2 = recording_status;
            }
        }


        public static void UpdateLB(ListBox LB, Object obj)
        {
            if (LB == null)
                return;

            if (LB.InvokeRequired)
            {
                UpdateLBDelegate ulb = new UpdateLBDelegate(UpdateLB);
                if (LB != null)
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

        public static void updateMediaStatusGui(int tuner, OpenTunerForm gui, MediaStatus new_status)
        {
            if (gui == null)
                return;

            if (gui.InvokeRequired)
            {
                updateMediaStatusGuiDelegate del = new updateMediaStatusGuiDelegate(updateMediaStatusGui);
                gui.Invoke(del, new object[] { tuner, gui, new_status });
            }
            else
            {
                if (tuner == 1)
                {
                    gui.prop_media_video_codec = new_status.VideoCodec;
                    gui.prop_media_video_resolution =
                        new_status.VideoWidth.ToString() + " x " + new_status.VideoHeight.ToString();

                    gui.prop_media_audio_codec = new_status.AudioCodec;
                    gui.prop_media_audio_rate = new_status.AudioRate.ToString() + " Hz, " +
                                                new_status.AudioChannels.ToString() + " channels";
                }
                else
                {
                    gui.prop_media_video_codec2 = new_status.VideoCodec;
                    gui.prop_media_video_resolution2 =
                        new_status.VideoWidth.ToString() + " x " + new_status.VideoHeight.ToString();

                    gui.prop_media_audio_codec2 = new_status.AudioCodec;
                    gui.prop_media_audio_rate2 = new_status.AudioRate.ToString() + " Hz, " +
                                                 new_status.AudioChannels.ToString() + " channels";
                }
            }
        }


        public static void updateTSStatusGui(int device, OpenTunerForm gui, TSStatus new_status)
        {
            if (gui == null)
                return;

            if (gui.InvokeRequired)
            {
                updateTSStatusGuiDelegate del = new updateTSStatusGuiDelegate(updateTSStatusGui);
                gui.Invoke(del, new object[] { device, gui, new_status });
            }
            else
            {
                if (device == 1)
                {
                    gui.prop_service_name = new_status.ServiceName;
                    gui.prop_service_provider_name = new_status.ServiceProvider;
                    gui.prop_null_packets = new_status.NullPacketsPerc.ToString() + "%";
                    gui.prop_null_packets_bar = Convert.ToInt32(new_status.NullPacketsPerc);
                }
                else
                {
                    gui.prop_service_name2 = new_status.ServiceName;
                    gui.prop_service_provider_name2 = new_status.ServiceProvider;
                    gui.prop_null_packets2 = new_status.NullPacketsPerc.ToString() + "%";
                    gui.prop_null_packets_bar2 = Convert.ToInt32(new_status.NullPacketsPerc);
                }
            }
        }

        public void updateNimStatusGui(OpenTunerForm gui, TunerStatus new_status)
        {
            if (gui == null)
                return;

            if (gui.InvokeRequired)
            {
                updateNimStatusGuiDelegate del = new updateNimStatusGuiDelegate(updateNimStatusGui);
                gui.Invoke(del, new object[] { gui, new_status });
            }
            else
            {
                if (mediaPlayer1Window != null)
                    mediaPlayer1Window.updateStatus(new_status);

                if (mediaPlayer2Window != null)
                    mediaPlayer2Window.updateStatus(new_status);

                // nim specific
                gui.prop_lpdc_errors = new_status.errors_ldpc_count.ToString();

                // tuner 1
                gui.prop_demodstate = Lookups.demod_state_lookup[new_status.T1P2_demod_status];
                double mer = Convert.ToDouble(new_status.T1P2_mer) / 10;
                gui.prop_mer = mer.ToString() + " dB";
                gui.prop_lnagain = new_status.T1P2_lna_gain.ToString();
                gui.prop_rf_input_level = new_status.T1P2_input_power_level.ToString() + " dB";
                gui.prop_symbol_rate = (new_status.T1P2_symbol_rate / 1000).ToString();
                gui.prop_modcod = new_status.T1P2_modcode.ToString();
                gui.prop_ber = new_status.T1P2_ber.ToString();
                gui.prop_freq_carrier_offset = new_status.T1P2_frequency_carrier_offset.ToString();
                gui.prop_req_freq = (mt.current_frequency_1 + mt.current_offset_A).ToString("N0") + " (" +
                                    mt.current_frequency_1.ToString("N0") + ")";
                gui.prop_rf_input = new_status.T1P2_rf_input;

                gui.prop_demodstate2 = Lookups.demod_state_lookup[new_status.T2P1_demod_status];
                double mer2 = Convert.ToDouble(new_status.T2P1_mer) / 10;
                gui.prop_mer2 = mer2.ToString() + " dB";
                gui.prop_lnagain2 = new_status.T2P1_lna_gain.ToString();
                gui.prop_rf_input_level2 = new_status.T2P1_input_power_level.ToString() + " dB";
                gui.prop_symbol_rate2 = (new_status.T2P1_symbol_rate / 1000).ToString();
                gui.prop_modcod2 = new_status.T2P1_modcode.ToString();
                gui.prop_ber2 = new_status.T2P1_ber.ToString();
                gui.prop_freq_carrier_offset2 = new_status.T2P1_frequency_carrier_offset.ToString();
                gui.prop_req_freq2 = (mt.current_frequency_2 + mt.current_offset_B).ToString("N0") + " (" +
                                     mt.current_frequency_1.ToString("N0") + ")";
                gui.prop_rf_input2 = new_status.T2P1_rf_input;

                try
                {
                    gui.prop_stream_format1 = Lookups
                        .stream_format_lookups[Convert.ToInt32(new_status.T1P2_stream_format)].ToString();

                    if (new_status.T1P2_stream_format == 1)
                    {
                        //bbframe_udp1.stream = true;
                    }
                }
                catch (Exception Ex)
                {
                    gui.prop_stream_format1 = "Unknown : " + new_status.T1P2_stream_format.ToString();
                }

                try
                {
                    gui.prop_stream_format2 = Lookups
                        .stream_format_lookups[Convert.ToInt32(new_status.T2P1_stream_format)].ToString();
                }
                catch (Exception Ex)
                {
                    gui.prop_stream_format2 = "Unknown : " + new_status.T2P1_stream_format.ToString();
                }

                // reset transport and media fields if no lock
                if (new_status.T1P2_demod_status < 2)
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

                if (new_status.T2P1_demod_status < 2)
                {
                    gui.prop_service_provider_name2 = "";
                    gui.prop_service_name2 = "";
                    gui.prop_null_packets2 = "";
                    gui.prop_null_packets_bar2 = 0;

                    gui.prop_media_audio_rate2 = "";
                    gui.prop_media_video_codec2 = "";
                    gui.prop_media_video_resolution2 = "";
                    gui.prop_media_audio_codec2 = "";
                }


                double dbmargin = 0;

                try
                {
                    switch (new_status.T1P2_demod_status)
                    {
                        case 2:
                            gui.prop_modcod = Lookups.modcod_lookup_dvbs2[new_status.T1P2_modcode];
                            dbmargin = (mer - Lookups.modcod_lookup_dvbs2_threshold[new_status.T1P2_modcode]);
                            gui.prop_db_margin = "D" + dbmargin.ToString("N1");
                            break;
                        case 3:
                            gui.prop_modcod = Lookups.modcod_lookup_dvbs[new_status.T1P2_modcode];
                            dbmargin = (mer - Lookups.modcod_lookup_dvbs_threshold[new_status.T1P2_modcode]);
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
                    gui.prop_modcod = "Err - " + new_status.T1P2_modcode.ToString();
                    gui.prop_db_margin = "";
                }

                try
                {
                    switch (new_status.T2P1_demod_status)
                    {
                        case 2:
                            gui.prop_modcod2 = Lookups.modcod_lookup_dvbs2[new_status.T2P1_modcode];
                            dbmargin = (mer2 - Lookups.modcod_lookup_dvbs2_threshold[new_status.T2P1_modcode]);
                            gui.prop_db_margin2 = "D" + dbmargin.ToString("N1");
                            break;
                        case 3:
                            gui.prop_modcod2 = Lookups.modcod_lookup_dvbs[new_status.T2P1_modcode];
                            dbmargin = (mer2 - Lookups.modcod_lookup_dvbs_threshold[new_status.T2P1_modcode]);
                            gui.prop_db_margin2 = "D" + dbmargin.ToString("N1");
                            break;
                        default:
                            gui.prop_modcod2 = "Unknown";
                            gui.prop_db_margin2 = "";
                            break;
                    }
                }
                catch (Exception Ex)
                {
                    gui.prop_modcod2 = "Err - " + new_status.T2P1_modcode.ToString();
                    gui.prop_db_margin2 = "";
                }

                /*
                // vlc marquee on fullscreen
                if (gui.isFullScreen)
                {
                    string marquee = lookups.demod_state_lookup[new_status.T1P2_demod_status] + " - " + gui.prop_db_margin + "(" + gui.prop_mer + ") - " + gui.prop_modcod + " - " + gui.prop_service_name;

                    MediaPlayer mediaPlayer = gui.prop_media_player;
                    mediaPlayer.SetMarqueeString(VideoMarqueeOption.Text, marquee);
                }
                */
            }
        }

        public OpenTunerForm()
        {
            ThreadPool.GetMinThreads(out int workers, out int ports);
            ThreadPool.SetMinThreads(workers + 6, ports + 6);

            //Thread.CurrentThread.CurrentUICulture = new CultureInfo("de-DE");

            load_settings();

            if (setting_language > 0)
            {
                switch (setting_language)
                {
                    case 1:
                        Thread.CurrentThread.CurrentUICulture = new CultureInfo("de-DE");
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

            greyPen.DashCap = DashCap.Round;
            greyPen.DashPattern = new float[] { 4.0F, 4.0F };

            InitializeComponent();

            OTColorChanger.OTChangeControlColors(this);

            if (Properties.Settings.Default.DarkMode)
            {
                OTMenuItemsColoring();
                menuStrip1.Renderer = new ToolStripRenderer();
                contextSpectrumMenu.Renderer = new ToolStripRenderer();
            }
            
            // test
            SoftBlink(lblrecordIndication1, Color.FromArgb(255, 255, 255), Color.Red, 2000, false);
            SoftBlink(lblRecordIndication2, Color.FromArgb(255, 255, 255), Color.Red, 2000, false);

            Console.WriteLine("Init done");
        }

       
        private void ChangeVideo(int video_number, bool start)
        {
            switch (video_number)
            {
                case 1:
                    if (start) start_video1();
                    else stop_video1();
                    break;
                case 2:
                    if (start) start_video2();
                    else stop_video2();
                    break;
            }
        }

        public void start_video1()
        {
            Console.WriteLine("Main: Starting Media Player 1");

            if (mt.ts_thread != null)
                mt.ts_thread.start_ts();

            if (media_player_1 != null)
                media_player_1.Play();
        }

        public void start_video2()
        {
            if (checkDisableVideo2.Checked == false)
            {
                Console.WriteLine("Main: Starting Media Player 2");

                if (mt.ts_thread2 != null)
                    mt.ts_thread2.start_ts();

                if (media_player_2 != null)
                    media_player_2.Play();
            }
        }


        public void stop_video1()
        {
            Console.WriteLine("Main: Stopping Media Player 1");

            if (media_player_1 != null)
                media_player_1.Stop();

            if (mt.ts_thread != null)
                mt.ts_thread.stop_ts();

            if (ts_recorder1 != null)
                ts_recorder1.record = false;

            if (ts_udp1 != null)
                ts_udp1.stream = false;

            //if (bbframe_udp1 != null)
            //    bbframe_udp1.stream = false;
        }

        public void stop_video2()
        {
            Console.WriteLine("Main: Stopping Media Player 2");

            if (mt.ts_thread2 != null)
                mt.ts_thread2.stop_ts();

            if (media_player_2 != null)
                media_player_2.Stop();

            if (ts_recorder2 != null)
                ts_recorder2.record = false;

            if (ts_udp2 != null)
                ts_udp2.stream = false;
        }


        public void parse_ts_data_callback(TSStatus ts_status)
        {
            updateTSStatusGui(1, this, ts_status);
        }

        public void parse_ts2_data_callback(TSStatus ts_status)
        {
            updateTSStatusGui(2, this, ts_status);
        }

        public void nim_status_feedback(TunerStatus nim_status)
        {
            bool T1P2locked = false;
            bool T2P1Locked = false;

            if (nim_status.T1P2_demod_status >= 2) T1P2locked = true;
            if (nim_status.T2P1_demod_status >= 2) T2P1Locked = true;

            updateNimStatusGui(this, nim_status);

            if (T1P2_prevLocked != T1P2locked)
            {
                Console.WriteLine("T1P2 - Lock State Change: " + T1P2_prevLocked.ToString() + "->" +
                                  T1P2locked.ToString());

                if (nim_status.T1P2_demod_status >= 2)
                {
                    start_video1();
                }
                else
                {
                    stop_video1();
                    //ftdi_hw.ftdi_ts_led(0, false);
                }

                T1P2_prevLocked = T1P2locked;
            }

            if (mt.ts_devices == 2)
            {
                if (T2P1_prevLocked != T2P1Locked)
                {
                    Console.WriteLine("T2P1 - Lock State Change: " + T2P1_prevLocked.ToString() + "->" +
                                      T2P1Locked.ToString());

                    if (nim_status.T2P1_demod_status >= 2)
                    {
                        start_video2();
                    }
                    else
                    {
                        stop_video2();
                        //ftdi_hw.ftdi_ts_led(0, false);
                    }

                    T2P1_prevLocked = T2P1Locked;
                }
            }
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
            Properties.Settings.Default.default_volume2 = trackVolume2.Value;

            // save current windows properties
            Properties.Settings.Default.window_width = Width;
            Properties.Settings.Default.window_height = Height;
            Properties.Settings.Default.window_x = Left;
            Properties.Settings.Default.window_y = Top;
            Properties.Settings.Default.main_splitter_pos = splitContainer1.SplitterDistance;

            Properties.Settings.Default.Save();

            try
            {
                stop_video1();
                if (mt.ts_devices == 2)
                    stop_video2();

                // close forms
                if (chatForm != null)
                    chatForm.Close();
                if (tuner1ControlForm != null)
                    tuner1ControlForm.Close();
                if (tuner2ControlForm != null)
                    tuner2ControlForm.Close();

                // close media players
                if (media_player_1 != null)
                    media_player_1.Close();
                if (media_player_2 != null)
                    media_player_2.Close();

                // close threads
                if (ts_recorder_1_t != null)
                    ts_recorder_1_t.Abort();
                if (ts_recorder_2_t != null)
                    ts_recorder_2_t.Abort();
                if (ts_udp_t1 != null)
                    ts_udp_t1.Abort();
                if (ts_udp_t2 != null)
                    ts_udp_t2.Abort();

                //if (bbframe_udp_t1 != null)
                //    bbframe_udp_t1.Abort();

                if (ts_parser_t != null)
                    ts_parser_t.Abort();
                if (ts_parser_2_t != null)
                    ts_parser_2_t.Abort();

                if (mt != null)
                {
                    mt.Close();
                }
            }
            catch (Exception Ex)
            {
                // we are closing, we don't really care about exceptions at this point
            }

            Application.Exit();
        }

        bool start_source()
        {
            bool error = false;

            Console.WriteLine("Main: Starting Source");

            if (setting_autodetect == true)
            {
                error = mt.Initialize(ChangeVideo, nim_status_feedback);
            }
            else
            {
                error = mt.Initialize(ChangeVideo, nim_status_feedback, true,
                    (manual_serial_devices.Count() > 0) ? manual_serial_devices[0] : "",
                    (manual_serial_devices.Count() > 1) ? manual_serial_devices[1] : "",
                    (manual_serial_devices.Count() > 2) ? manual_serial_devices[2] : "");
            }

            Text = Text += " - " + mt.HardwareDevice;

            return error;
        }

        private void btnConnectTuner_Click(object sender, EventArgs e)
        {
            // start source
            start_source();

            TSParserThread ts_parser_thread = new TSParserThread(parse_ts_data_callback, mt.ts_thread);
            ts_parser_t = new Thread(ts_parser_thread.worker_thread);
            ts_parser_t.Start();

            if (mt.ts_devices == 2)
            {
                TSParserThread ts_parser_thread2 = new TSParserThread(parse_ts2_data_callback, mt.ts_thread2);
                ts_parser_2_t = new Thread(ts_parser_thread2.worker_thread);
                ts_parser_2_t.Start();
            }

            // TS recorder Thread
            ts_recorder1 = new TSRecorderThread(mt.ts_thread, setting_snapshot_path, "t1");
            ts_recorder1.onRecordStatusChange += Ts_recorder_onRecordStatusChange;
            ts_recorder_1_t = new Thread(ts_recorder1.worker_thread);
            ts_recorder_1_t.Start();

            // TS udp thread - tuner 1
            ts_udp1 = new TSUDPThread(mt.ts_thread, setting_udp_address1, setting_udp_port1);
            ts_udp_t1 = new Thread(ts_udp1.worker_thread);
            ts_udp_t1.Start();

            if (mt.ts_devices == 2)
            {
                // TS udp thread - tuner 2
                ts_udp2 = new TSUDPThread(mt.ts_thread2, setting_udp_address2, setting_udp_port2);
                ts_udp_t2 = new Thread(ts_udp2.worker_thread);
                ts_udp_t2.Start();

                // TS recorder Thread
                ts_recorder2 = new TSRecorderThread(mt.ts_thread2, setting_snapshot_path, "t2");
                ts_recorder2.onRecordStatusChange += Ts_recorder_onRecordStatusChange;
                ts_recorder_2_t = new Thread(ts_recorder2.worker_thread);
                ts_recorder_2_t.Start();
            }

            if (setting_mediaplayer_1 == 0)
            {
                if (setting_windowed_mediaPlayer1 == true)
                {
                    mediaPlayer1Window = new VideoViewForm(setting_mediaplayer_1, 1);
                    media_player_1 = new VLCMediaPlayer(mediaPlayer1Window.vlcPlayer);
                    mediaPlayer1Window.Show();
                    ffmpegVideoView1.Visible = false;
                    videoView1.Visible = false;
                }
                else
                {
                    media_player_1 = new VLCMediaPlayer(videoView1);
                    ffmpegVideoView1.Visible = false;
                }

                groupTuner1.Text = "Tuner 1 - VLC";
            }
            else
            {
                if (setting_windowed_mediaPlayer1 == true)
                {
                    mediaPlayer1Window = new VideoViewForm(setting_mediaplayer_1, 1);
                    media_player_1 = new FFMPEGMediaPlayer(mediaPlayer1Window.ffmpegPlayer);
                    mediaPlayer1Window.Show();
                    ffmpegVideoView1.Visible = false;
                    videoView1.Visible = false;
                }
                else
                {
                    media_player_1 = new FFMPEGMediaPlayer(ffmpegVideoView1);
                    videoView1.Visible = false;
                    groupTuner1.Text = "Tuner 1 - FFMPEG";
                }
            }

            media_player_1.onVideoOut += MediaPlayer_Vout;
            media_player_1.Initialize(mt.ts_data_queue);

            if (mt.ts_devices == 2)
            {
                if (setting_mediaplayer_2 == 0)
                {
                    if (setting_windowed_mediaPlayer2 == true)
                    {
                        mediaPlayer2Window = new VideoViewForm(setting_mediaplayer_2, 2);
                        media_player_2 = new VLCMediaPlayer(mediaPlayer2Window.vlcPlayer);
                        mediaPlayer2Window.Show();
                        ffmpegVideoView2.Visible = false;
                        videoView2.Visible = false;
                    }
                    else
                    {
                        media_player_2 = new VLCMediaPlayer(videoView2);
                        ffmpegVideoView2.Visible = false;
                    }

                    groupTuner2.Text = "Tuner 2 - VLC";
                }
                else
                {
                    if (setting_windowed_mediaPlayer2 == true)
                    {
                        mediaPlayer2Window = new VideoViewForm(setting_mediaplayer_2, 2);
                        media_player_2 = new FFMPEGMediaPlayer(mediaPlayer2Window.ffmpegPlayer);
                        mediaPlayer1Window.Show();
                        ffmpegVideoView2.Visible = false;
                        videoView2.Visible = false;
                    }
                    else
                    {
                        media_player_2 = new FFMPEGMediaPlayer(ffmpegVideoView2);
                        videoView2.Visible = false;
                    }

                    groupTuner2.Text = "Tuner 2 - FFMPEG";
                }

                media_player_2.onVideoOut += MediaPlayer_Vout2;
                media_player_2.Initialize(mt.ts_data_queue2);
            }
            else
            {
                videoPlayersSplitter.Panel2Collapsed = true;
                videoPlayersSplitter.Panel2.Hide();
                groupTuner2.Visible = false;
                tuner2ToolStripMenuItem.Visible = false;
            }

            // if we have either 1 of the 2 video players windowed then increase the size on the remaining one
            if (setting_windowed_mediaPlayer1 == true && setting_windowed_mediaPlayer2 == false)
            {
                videoPlayersSplitter.Panel1Collapsed = true;
                videoPlayersSplitter.Panel1.Hide();
            }

            if (setting_windowed_mediaPlayer1 == false && setting_windowed_mediaPlayer2 == true)
            {
                videoPlayersSplitter.Panel2Collapsed = true;
                videoPlayersSplitter.Panel2.Hide();
            }

            if (mt.ts_devices == 1)
            {
                tuner2ToolStripMenuItem.Visible = false;
            }

            // temporary to prevent multiple connection attempts
            // todo: deal with this properly
            lblConnected.Text = "Connected";
            lblConnected.ForeColor = Color.Coral;
            menuConnect.Enabled = false;
            lblConnected.Enabled = false;
        }

        private void MediaPlayer_Vout2(object sender, MediaStatus media_status)
        {
            //ftdi_hw.ftdi_ts_led(1, true);

            updateMediaStatusGui(2, this, media_status);

            if (media_player_2 != null)
                media_player_2.SetVolume(rxVolume2);

            if (enableUDPOutputToolStripMenuItem1.Checked)
            {
                if (ts_udp2 != null)
                {
                    ts_udp2.stream = true;
                }
            }
        }

        private void Ts_recorder_onRecordStatusChange(object sender, bool e)
        {
            TSRecorderThread whichRecorder = (TSRecorderThread)sender;
            updateRecordingStatus(this, e, whichRecorder.id);
        }

        private void LibVLC_Log(object sender, LogEventArgs e)
        {
            debug("VLC Log: " + e.FormattedLog + "," + e.Level);
        }

        private void MediaPlayer_Vout(object sender, MediaStatus media_status)
        {
            //ftdi_hw.ftdi_ts_led(0, true);

            updateMediaStatusGui(1, this, media_status);

            if (media_player_1 != null)
                media_player_1.SetVolume(rxVolume);

            if (recordAllToolStripMenuItem.Checked)
            {
                if (ts_recorder1 != null)
                    ts_recorder1.record = true; // recording will automatically stop when lock is lost
            }

            if (enableUDPOutputToolStripMenuItem.Checked)
            {
                if (ts_udp1 != null)
                {
                    ts_udp1.stream = true;
                }
            }
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

                channels[n] = new Rectangle(Convert.ToInt32(pos * spectrum_wScale) - (w / 2), offset - (split / 2) - 3,
                    w, split - 2);
                n++;
            }

            //draw blocks
            for (int i = 0; i < count; i++)
            {
                tmp2.FillRectangles(bandplanBrush, new RectangleF[] { channels[i] }); //x,y,w,h
            }
        }

        private void drawspectrum_signals(List<signal.Sig> signals)
        {
            float spectrum_w = spectrum.Width;
            float spectrum_wScale = spectrum_w / 922;

            lock (list_lock) //hopefully lock signals list while drawing
            {
                //draw the text for each signal found
                foreach (signal.Sig s in signals)
                {
                    tmp.DrawString(
                        s.callsign + "\n" + s.frequency.ToString("#.00") + "\n " + (s.sr * 1000).ToString("#Ks"),
                        new Font("Tahoma", 10), Brushes.White,
                        new PointF(Convert.ToSingle((s.fft_centre * spectrum_wScale) - (25)),
                            (255 - Convert.ToSingle(s.fft_strength + 50))));
                }
            }

            try
            {
                Invoke(new MethodInvoker(delegate ()
                {
                    spectrum.Image = bmp;
                    spectrum.Update();
                }));
            }
            catch (Exception Ex)
            {
            }
        }

        private void drawspectrum(UInt16[] fft_data)
        {
            tmp.Clear(Color.Black); //clear canvas


            int spectrum_h = spectrum.Height - bandplan_height;
            float spectrum_w = spectrum.Width;
            float spectrum_wScale = spectrum_w / 922;

            int i = 1;
            int y = 0;

            for (i = 1; i <= 4; i++)
            {
                y = spectrum_h - ((i * (spectrum_h / 4)) - (spectrum_h / 6));
                tmp.DrawLine(greyPen, 10, y, spectrum_w - 10, y);
            }

            PointF[] points = new PointF[fft_data.Length - 2];

            for (i = 1; i < fft_data.Length - 3; i++) //ignore padding?
            {
                PointF point = new PointF(i * spectrum_wScale, 255 - fft_data[i] / 255);
                points[i] = point;
            }

            points[0] = new PointF(0, 255);
            points[points.Length - 1] = new PointF(spectrum_w, 255);

            if (spectrumTunerHighlight > 0)
                tmp.FillRectangle((spectrumTunerHighlight == 1 ? tuner1Brush : tuner2Brush),
                    new RectangleF(0, (spectrumTunerHighlight == 1 ? 0 : spectrum_h / 2), spectrum_w,
                        mt.ts_devices == 1 ? spectrum_h : spectrum_h / 2));

            //tmp.DrawPolygon(greenpen, points);
            SolidBrush spectrumBrush = new SolidBrush(Color.Blue);

            LinearGradientBrush linGrBrush = new LinearGradientBrush(
                new Point(0, 0),
                new Point(0, 255),
                Color.FromArgb(255, 255, 99, 132), // Opaque red
                Color.FromArgb(255, 54, 162, 235)); // Opaque blue

            tmp.FillPolygon(linGrBrush, points);

            tmp.DrawImage(bmp2, 0, 255 - bandplan_height); //bandplan

            y = 0;
            int y_offset = 0;
            ;

            for (int tuner = 0; tuner < mt.ts_devices; tuner++)
            {
                y = spectrum_h - ((spectrum_h / 2) * tuner + 3);
                y_offset = (spectrum_h / 2) / 2 + 10;

                //draw block showing signal selected
                if (rx_blocks[tuner, 0] > 0)
                {
                    //tmp.FillRectangles(shadowBrush, new RectangleF[] { new System.Drawing.Rectangle(Convert.ToInt32((rx_blocks[0] * spectrum_wScale) - ((rx_blocks[1] * spectrum_wScale) / 2)), 1, Convert.ToInt32(rx_blocks[1] * spectrum_wScale), (255) - 4) });
                    tmp.FillRectangles(shadowBrush,
                        new RectangleF[]
                        {
                            new Rectangle(
                                Convert.ToInt32((rx_blocks[tuner, 0] - (rx_blocks[tuner, 1] / 2)) * spectrum_wScale),
                                spectrum_h - y + 1, Convert.ToInt32((rx_blocks[tuner, 1] * spectrum_wScale)),
                                (mt.ts_devices == 1 ? spectrum_h : spectrum_h / 2) - 4)
                        });
                }
            }


            tmp.DrawString(InfoText, new Font("Tahoma", 15), Brushes.White, new PointF(10, 10));
            tmp.DrawString(TX_Text, new Font("Tahoma", 15), Brushes.Red, new PointF(70, spectrum.Height - 50)); //dh3cs


            //drawspectrum_signals(sigs.detect_signals(fft_data));
            sigs.detect_signals(fft_data);

            // draw over power
            foreach (var sig in sigs.signalsData)
            {
                if (sig.overpower)
                {
                    tmp.FillRectangles(overpowerBrush,
                        new RectangleF[]
                        {
                            new Rectangle(
                                Convert.ToInt16(sig.fft_centre * spectrum_wScale) -
                                (Convert.ToInt16((sig.fft_stop - sig.fft_start) * spectrum_wScale) / 2), 1,
                                Convert.ToInt16((sig.fft_stop - sig.fft_start) * spectrum_wScale), (255) - 4)
                        });
                }
            }

            if (spectrumTunerHighlight > 0)
                tmp.DrawString("RX" + spectrumTunerHighlight.ToString(), new Font("Tahoma", 15), Brushes.White,
                    new PointF(10, (spectrum_h / 2) + (spectrumTunerHighlight == 1 ? -30 : 10)));

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
                    Clipboard.SetText(tx_freq); //DATV-Easy in MHz
                    TX_Text = " TX: " + tx_freq;
                }
            }
            else
            {
                selectSignal(X, Y);
            }
        }


        // quick tune functions - From https://github.com/m0dts/QO-100-WB-Live-Tune - Rob Swinbank
        private void selectSignal(int X, int Y)
        {
            float spectrum_w = spectrum.Width;
            float spectrum_wScale = spectrum_w / 922;
            int spectrum_h = spectrum.Height - bandplan_height;

            int rx = 0;

            if (mt.ts_devices == 2)
            {
                if (Y > spectrum_h / 2)
                    rx = 1;
            }

            debug("Select Signal");
            try
            {
                foreach (signal.Sig s in sigs.signals)
                {
                    if ((X / spectrum_wScale) > s.fft_start & (X / spectrum_wScale) < s.fft_stop)
                    {
                        sigs.set_tuned(s, rx);
                        rx_blocks[rx, 0] = Convert.ToInt16(s.fft_centre);
                        rx_blocks[rx, 1] = Convert.ToInt16((s.fft_stop) - (s.fft_start));
                        UInt32 freq = Convert.ToUInt32((s.frequency) * 1000);
                        UInt32 sr = Convert.ToUInt32((s.sr * 1000.0));

                        debug("Freq: " + freq.ToString());
                        debug("SR: " + sr.ToString());

                        UInt32 lo = 0;

                        if (rx == 0) // tuner 1
                        {
                            if (mt.current_rf_input_1 == nim.NIM_INPUT_TOP)
                                lo = Convert.ToUInt32(mt.current_offset_A);
                            else
                                lo = Convert.ToUInt32(mt.current_offset_B);
                        }
                        else
                        {
                            if (mt.current_rf_input_2 == nim.NIM_INPUT_TOP)
                                lo = Convert.ToUInt32(mt.current_offset_A);
                            else
                                lo = Convert.ToUInt32(mt.current_offset_B);
                        }

                        if (mt.ts_devices == 2)
                            change_frequency_with_lo((byte)(rx + 1), freq, lo, sr);
                        else
                            change_frequency_with_lo(1, freq, lo, sr);
                    }
                }
            }
            catch (Exception Ex)
            {
            }
        }

        int spectrumTunerHighlight = 0;

        public void spectrum_MouseMove(object sender, MouseEventArgs e)
        {
            get_bandplan_TX_freq(e.X, e.Y); // dh3cs

            if (mt.ts_devices == 2)
            {
                if (e.Y <= spectrum.Height / 2)
                    spectrumTunerHighlight = 1;
                else
                    spectrumTunerHighlight = 2;
            }
            else
            {
                spectrumTunerHighlight = 1;
            }
        }

        // moved to separate function, to use by right click in spectrum,  dh3cs 
        private string get_bandplan_TX_freq(int x, int y) // returns TX-Freq in MHz from the rectangle in Bandplan
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
                            if (y - (spectrum.Height - bandplan_height) >= ch.Location.Y - (ch.Height / 2) + 3 &
                                y - (spectrum.Height - bandplan_height) <= ch.Location.Y + (ch.Height / 2) + 3)
                            {
                                tx_freq_MHz = indexedbandplan[n].Element("s-freq").Value;
                                InfoText = " Dn: " + indexedbandplan[n].Element("x-freq").Value + "  SR: " +
                                           indexedbandplan[n].Element("name").Value + Environment.NewLine
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
            setting_default_lo_value_1 = Properties.Settings.Default.default_lo_B;
            setting_default_lo_value_2 = Properties.Settings.Default.default_lo_A;
            setting_enable_spectrum = Properties.Settings.Default.enable_qo100_spectrum;
            setting_snapshot_path = Properties.Settings.Default.media_snapshot_path;
            setting_language = Properties.Settings.Default.language;
            setting_default_volume = Properties.Settings.Default.default_volume;
            setting_default_volume2 = Properties.Settings.Default.default_volume2;
            setting_chat_font_size = Properties.Settings.Default.wbchat_font_size;

            //setting_chat_font = Properties.Settings.Default.wbchat_font;
            setting_chat_width = Properties.Settings.Default.wbchat_width;
            setting_chat_height = Properties.Settings.Default.wbchat_height;
            setting_enable_chatform = Properties.Settings.Default.wbchat_enable;
            setting_enable_darkmode = Properties.Settings.Default.DarkMode;


            setting_window_width = Properties.Settings.Default.window_width;
            setting_window_height = Properties.Settings.Default.window_height;
            setting_window_x = Properties.Settings.Default.window_x;
            setting_window_y = Properties.Settings.Default.window_y;
            setting_main_splitter_position = Properties.Settings.Default.main_splitter_pos;

            setting_mediaplayer_1 = Properties.Settings.Default.mediaplayer_tuner1; // 0 = vlc, 1 = ffmpeg
            setting_mediaplayer_2 = Properties.Settings.Default.mediaplayer_tuner2; // 0 = vlc, 1 = ffmpeg

            setting_udp_address1 = Properties.Settings.Default.udp_address1;
            setting_udp_port1 = Properties.Settings.Default.udp_port1;

            setting_udp_address2 = Properties.Settings.Default.udp_address2;
            setting_udp_port2 = Properties.Settings.Default.udp_port2;

            setting_windowed_mediaPlayer1 = Properties.Settings.Default.windowed_player1;
            setting_windowed_mediaPlayer2 = Properties.Settings.Default.windowed_player2;

            setting_sigreport_template = Properties.Settings.Default.signreport_template;

            setting_tuner1_startfreq = Properties.Settings.Default.tuner1_start_freq;
            setting_tuner2_startfreq = Properties.Settings.Default.tuner2_start_freq;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            debug("System Culture Setting: " + CultureInfo.CurrentCulture.Name);
            debug("Settings: Restore Last Volume: " + setting_default_volume.ToString() + "%");
            trackVolume.Value = setting_default_volume;
            debug("Settings: Restore Last Volume2: " + setting_default_volume2.ToString() + "%");
            trackVolume2.Value = setting_default_volume2;
            debug("Settings: Default Offset A: " + setting_default_lo_value_1.ToString());
            mt.current_offset_A = setting_default_lo_value_1;
            debug("Settings: Default Offset B: " + setting_default_lo_value_2.ToString());
            mt.current_offset_B = setting_default_lo_value_2;

            if (setting_snapshot_path.Length == 0)
                setting_snapshot_path = AppDomain.CurrentDomain.BaseDirectory;

            debug("Settings: Snapshot Path: " + setting_snapshot_path);

            debug("Settings MediaPlayer 1 : " + (setting_mediaplayer_1 == 0 ? "VLC" : "FFMPEG"));
            debug("Settings MediaPlayer 2 : " + (setting_mediaplayer_2 == 0 ? "VLC" : "FFMPEG"));

            offToolStripMenuItem.Checked = false;
            vertical13VToolStripMenuItem.Checked = false;
            horizontal18VToolStripMenuItem.Checked = false;

            switch (setting_default_lnb_supply)
            {
                case 0:
                    mt.current_enable_lnb_supply = false;
                    mt.current_enable_horiz_supply = false;
                    offToolStripMenuItem.Checked = true;
                    break;
                case 1:
                    mt.current_enable_lnb_supply = true;
                    mt.current_enable_horiz_supply = false;
                    vertical13VToolStripMenuItem.Checked = true;
                    break;
                case 2:
                    mt.current_enable_lnb_supply = true;
                    mt.current_enable_horiz_supply = true;
                    horizontal18VToolStripMenuItem.Checked = true;
                    break;
            }

            debug("Settings: Enable LNB Supply: " + mt.current_enable_lnb_supply.ToString());

            if (mt.current_enable_lnb_supply)
            {
                debug("Settings: Enable Vert Supply: " + (!mt.current_enable_horiz_supply).ToString());
                debug("Settings: Enable Horiz Supply: " + mt.current_enable_horiz_supply.ToString());
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

                if (arg.StartsWith("MANUAL="))
                {
                    string param = arg.Substring(7);
                    Console.WriteLine("Manual Device Specifed! : " + param);

                    manual_serial_devices = param.Split(',');

                    for (int c = 0; c < manual_serial_devices.Length; c++)
                    {
                        switch (c)
                        {
                            case 0:
                                Console.WriteLine("I2C: " + manual_serial_devices[c].ToString());
                                break;
                            case 1:
                                Console.WriteLine("Tuner 1 TS: " + manual_serial_devices[c].ToString());
                                break;
                            case 2:
                                Console.WriteLine("Tuner 2 TS: " + manual_serial_devices[c].ToString());
                                break;
                        }
                    }

                    setting_autodetect = false;
                }
            }

            if (setting_enable_chatform)
            {
                qO100WidebandChatToolStripMenuItem.Visible = true;
                chatForm = new WideBandChatForm(setting_chat_font_size);

                lblChatSigReport.Visible = true;

                if (setting_chat_width > -1 && setting_chat_height > -1)
                {
                    chatForm.Size = new Size(setting_chat_height, setting_chat_width);
                }
            }
            else
            {
                qO100WidebandChatToolStripMenuItem.Visible = false;
            }


            if (setting_enable_spectrum)
            {
                debug("Settings: QO-100 Spectrum Enabled");

                bmp2 = new Bitmap(spectrum.Width, bandplan_height); //bandplan
                bmp = new Bitmap(spectrum.Width, height + 20);
                tmp = Graphics.FromImage(bmp);
                tmp2 = Graphics.FromImage(bmp2);

                try
                {
                    bandplan = XElement.Load(Path.GetDirectoryName(Application.ExecutablePath) +
                                             @"\ResourceFiles\bandplan.xml");
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
                string title = Text;
                sock.start();
                Text = title;

                DoubleBuffered = true;

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

            Console.WriteLine("Restoring Window Positions:");
            Console.WriteLine(" Size: (" + setting_window_height.ToString() + "," + setting_window_width.ToString() +
                              ")");
            Console.WriteLine(" Position: (" + setting_window_x.ToString() + "," + setting_window_y.ToString() + ")");

            Height = setting_window_height;
            Width = setting_window_width;

            Left = setting_window_x;
            Top = setting_window_y;

            splitContainer1.SplitterDistance = setting_main_splitter_position;

            load_stored_frequencies();
            rebuild_stored_frequencies();

            load_external_tools();
            rebuild_external_tools();

            // tuner control windows
            TunerChangeCallback tuner1Callback = new TunerChangeCallback(tuner1_change_callback);
            TunerChangeCallback tuner2Callback = new TunerChangeCallback(tuner2_change_callback);

            tuner1ControlForm = new TunerControlForm(tuner1Callback);
            tuner2ControlForm = new TunerControlForm(tuner2Callback);

            tuner1ControlForm.Text = "Tuner 1";
            tuner2ControlForm.Text = "Tuner 2";

            tuner1ControlForm.set_offset(mt.current_offset_A, mt.current_offset_B);
            tuner2ControlForm.set_offset(mt.current_offset_A, mt.current_offset_B);

            Console.WriteLine("Load Done");

            // this needs to go last
            if (setting_auto_connect)
            {
                btnConnectTuner_Click(this, e);
            }
        }

        void tuner1_change_callback(uint freq, uint rf_input, uint symbol_rate)
        {
            mt.change_frequency(1, freq, symbol_rate, mt.current_enable_lnb_supply, mt.current_enable_horiz_supply,
                rf_input, mt.current_tone_22kHz_P1);
        }

        void tuner2_change_callback(uint freq, uint rf_input, uint symbol_rate)
        {
            mt.change_frequency(2, freq, symbol_rate, mt.current_enable_lnb_supply, mt.current_enable_horiz_supply,
                rf_input, mt.current_tone_22kHz_P1);
        }

        private void rebuild_external_tools()
        {
            if (external_tools.Count == 0)
            {
                externalToolsToolStripMenuItem1.Visible = false;
                return;
            }

            externalToolsToolStripMenuItem1.Visible = true;

            for (int c = 0; c < external_tools.Count; c++)
            {
                ToolStripMenuItem et_menu = new ToolStripMenuItem(external_tools[c].ToolName);
                et_menu.Tag = c;
                et_menu.Click += Et_menu_Click;

                externalToolsToolStripMenuItem1.DropDownItems.Add(et_menu);
            }
        }

        private void Et_menu_Click(object sender, EventArgs e)
        {
            int tag = Convert.ToInt32(((ToolStripMenuItem)(sender)).Tag);
            Console.WriteLine(tag.ToString());

            if (tag < external_tools.Count)
            {
                //MessageBox.Show("Run " + external_tools[tag].ToolName);
                try
                {
                    if (external_tools[tag].EnableUDP1)
                    {
                        if (ts_udp1 != null)
                        {
                            ts_udp1.stream = true;
                            enableUDPOutputToolStripMenuItem.Checked = true;
                        }
                    }

                    if (external_tools[tag].EnableUDP2)
                    {
                        if (ts_udp2 != null)
                        {
                            ts_udp2.stream = true;
                            enableUDPOutputToolStripMenuItem1.Checked = true;
                        }
                    }

                    string parameters = external_tools[tag].ToolParameters;

                    parameters = parameters.Replace("{UDP1PORT}", setting_udp_port1.ToString());
                    parameters = parameters.Replace("{UDP2PORT}", setting_udp_port2.ToString());
                    parameters = parameters.Replace("{UDP1URL}", setting_udp_address1.ToString());
                    parameters = parameters.Replace("{UDP2URL}", setting_udp_address2.ToString());

                    Process.Start(external_tools[tag].ToolPath, parameters);
                }
                catch (Exception Ex)
                {
                    MessageBox.Show("Error running external tool: " + Ex.Message);
                }
            }
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
                ToolStripMenuItem sf_menu = new ToolStripMenuItem(stored_frequencies[c].Name + " (" +
                                                                  stored_frequencies[c].Frequency + ")( Tuner " +
                                                                  (stored_frequencies[c].DefaultTuner + 1).ToString() +
                                                                  ")");
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
            if (mt != null)
                mt.change_frequency(Convert.ToByte(sf.DefaultTuner + 1), sf.Frequency - sf.Offset, sf.SymbolRate,
                    mt.current_enable_lnb_supply, mt.current_enable_horiz_supply, (uint)sf.RFInput,
                    mt.current_tone_22kHz_P1);
        }


        void change_frequency_with_lo(byte tuner, UInt32 freq, UInt32 lo, UInt32 sr)
        {
            if (mt != null)
                mt.change_frequency(tuner, freq - lo, sr, mt.current_enable_lnb_supply, mt.current_enable_horiz_supply,
                    tuner == 1 ? mt.current_rf_input_1 : mt.current_rf_input_2, mt.current_tone_22kHz_P1);
        }

        void change_rf_input(byte tuner, uint rf_input)
        {
            mt.change_frequency(tuner, tuner == 1 ? mt.current_frequency_1 : mt.current_frequency_2,
                tuner == 1 ? mt.current_sr_1 : mt.current_sr_2, mt.current_enable_lnb_supply,
                mt.current_enable_horiz_supply, rf_input, mt.current_tone_22kHz_P1);
        }

        private void load_external_tools()
        {
            string tools_file = AppDomain.CurrentDomain.BaseDirectory + "\\tools.json";
            string json = "";

            try
            {
                json = File.ReadAllText(tools_file);
                external_tools = JsonConvert.DeserializeObject<List<ExternalTool>>(json);
            }
            catch (Exception Ex)
            {
                Console.WriteLine("Error reading tools.json - file missing, or wrong format :" + Ex.Message);
            }
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
            catch (Exception Ex)
            {
                Console.WriteLine("Error reading frequencies.json - file missing, or wrong format :" + Ex.Message);
            }
        }

        private void save_external_tools()
        {
            string json = JsonConvert.SerializeObject(external_tools, Formatting.Indented);

            string tools_file = AppDomain.CurrentDomain.BaseDirectory + "\\tools.json";

            try
            {
                File.WriteAllText(tools_file, json);
            }
            catch (Exception Ex)
            {
                Console.WriteLine("Error writing tools file: " + Ex.Message);
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
            catch (Exception Ex)
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
                bmp2 = new Bitmap(spectrum.Width, bandplan_height); //bandplan
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
        }

        private void trackVolume_ValueChanged(object sender, EventArgs e)
        {
            rxVolume = Convert.ToByte(trackVolume.Value);
            if (media_player_1 != null)
                media_player_1.SetVolume(rxVolume);
            lblVolume.Text = rxVolume.ToString() + " %";
        }

        private void TakeSnapshot(int tuner)
        {
            // get path
            string path = setting_snapshot_path;
            string filename = DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss") + ".png";


            if (tuner == 1)
            {
                if (lblServiceName.Text.Length > 0 && tuner == 1)
                    filename = lblServiceName.Text.ToString() + "_" + filename;

                filename = filename.Replace(" ", "");

                if (media_player_1 != null)
                    media_player_1.SnapShot(path + filename);
            }
            else
            {
                if (lblServiceName2.Text.Length > 0 && tuner == 2)
                    filename = lblServiceName2.Text.ToString() + "_" + filename;

                filename = filename.Replace(" ", "");

                if (media_player_2 != null)
                    media_player_2.SnapShot(path + filename);
            }
        }

        private void openTunerWebsiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.zr6tg.co.za/open-tuner/");
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            Process.Start("https://batc.org.uk/");
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingsForm settings_form = new SettingsForm();

            settings_form.txtDefaultLO.Text = setting_default_lo_value_1.ToString();
            settings_form.txtDefaultLO2.Text = setting_default_lo_value_2.ToString();
            settings_form.comboDefaultLNB.SelectedIndex = setting_default_lnb_supply;
            settings_form.txtSnapshotPath.Text = setting_snapshot_path;
            settings_form.checkEnableSpectrum.Checked = setting_enable_spectrum;
            settings_form.checkEnableChat.Checked = setting_enable_chatform;
            settings_form.darkModeCheckBox.Checked = setting_enable_darkmode;
            settings_form.comboLanguage.SelectedIndex = setting_language;
            settings_form.comboMediaPlayer1.SelectedIndex = setting_mediaplayer_1;
            settings_form.comboMediaPlayer2.SelectedIndex = setting_mediaplayer_2;

            settings_form.textUDPAddress.Text = setting_udp_address1;
            settings_form.numUdpPort.Value = setting_udp_port1;

            settings_form.textUDPAddress2.Text = setting_udp_address2;
            settings_form.numUdpPort2.Value = setting_udp_port2;

            settings_form.checkWindowed1.Checked = setting_windowed_mediaPlayer1;
            settings_form.checkWindowed2.Checked = setting_windowed_mediaPlayer2;

            settings_form.txtSigReportTemplate.Text = setting_sigreport_template;

            settings_form.numChatFontSize.Value = setting_chat_font_size;

            settings_form.comboTuner1Start.Items.Add("Default");
            settings_form.comboTuner2Start.Items.Add("Default");

            settings_form.comboTuner1Start.SelectedIndex = 0;
            settings_form.comboTuner2Start.SelectedIndex = 0;

            for (int c = 0; c < stored_frequencies.Count; c++)
            {
                if (stored_frequencies[c].DefaultTuner == 0)
                {
                    int index = settings_form.comboTuner1Start.Items.Add(stored_frequencies[c].Name);

                    if (stored_frequencies[c].Name == setting_tuner1_startfreq)
                        settings_form.comboTuner1Start.SelectedIndex = index;
                }
                else
                {
                    int index = settings_form.comboTuner2Start.Items.Add(stored_frequencies[c].Name);

                    if (stored_frequencies[c].Name == setting_tuner2_startfreq)
                        settings_form.comboTuner2Start.SelectedIndex = index;
                }
            }


            if (settings_form.ShowDialog() == DialogResult.OK)
            {
                //Properties.Settings.Default.wbchat_font = settings_form.currentChatFont;
                Properties.Settings.Default.wbchat_font_size = Convert.ToInt32(settings_form.numChatFontSize.Value);
                Properties.Settings.Default.DarkMode = settings_form.darkModeCheckBox.Checked;

                Properties.Settings.Default.default_lnb_supply =
                    Convert.ToByte(settings_form.comboDefaultLNB.SelectedIndex);
                Properties.Settings.Default.default_lo_B = Convert.ToInt32(settings_form.txtDefaultLO.Text);
                Properties.Settings.Default.default_lo_A = Convert.ToInt32(settings_form.txtDefaultLO2.Text);
                Properties.Settings.Default.enable_qo100_spectrum = settings_form.checkEnableSpectrum.Checked;
                Properties.Settings.Default.wbchat_enable = settings_form.checkEnableChat.Checked;
                Properties.Settings.Default.media_snapshot_path = settings_form.txtSnapshotPath.Text;
                Properties.Settings.Default.mediaplayer_tuner1 = settings_form.comboMediaPlayer1.SelectedIndex;
                Properties.Settings.Default.mediaplayer_tuner2 = settings_form.comboMediaPlayer2.SelectedIndex;
                Properties.Settings.Default.udp_address1 = settings_form.textUDPAddress.Text;
                Properties.Settings.Default.udp_port1 = Convert.ToInt32(settings_form.numUdpPort.Value);

                Properties.Settings.Default.udp_address2 = settings_form.textUDPAddress2.Text;
                Properties.Settings.Default.udp_port2 = Convert.ToInt32(settings_form.numUdpPort2.Value);

                setting_language = settings_form.comboLanguage.SelectedIndex;

                Properties.Settings.Default.language = setting_language;

                Properties.Settings.Default.windowed_player1 = settings_form.checkWindowed1.Checked;
                Properties.Settings.Default.windowed_player2 = settings_form.checkWindowed2.Checked;
                Properties.Settings.Default.signreport_template = settings_form.txtSigReportTemplate.Text;

                Properties.Settings.Default.tuner1_start_freq = settings_form.comboTuner1Start.Text;
                Properties.Settings.Default.tuner2_start_freq = settings_form.comboTuner2Start.Text;

                setting_sigreport_template = settings_form.txtSigReportTemplate.Text;

                Properties.Settings.Default.Save();

                load_settings();
            }
        }

        private void lblConnected_Click(object sender, EventArgs e)
        {
            if (!mt.hardware_connected)
                btnConnectTuner_Click(this, e);
        }

        private void menuFullScreen_Click(object sender, EventArgs e)
        {
        }

        private void qO100WidebandChatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            chatForm.Show();
            chatForm.Focus();
        }

        private void lblServiceName_TextChanged(object sender, EventArgs e)
        {
            if (setting_enable_spectrum)
            {
                // we have decoded a callsign
                string callsign = lblServiceName.Text;
                int offset = 0;

                //Int32.TryParse(current, out offset);
                if (mt.current_rf_input_1 == nim.NIM_INPUT_TOP)
                    offset = mt.current_offset_A;
                else
                    offset = mt.current_offset_B;

                if (callsign.Length > 0)
                {
                    double freq = mt.current_frequency_1 + offset;
                    freq = freq / 1000;
                    float sr = mt.current_sr_1;

                    debug("New Callsign: " + callsign + "," + freq.ToString() + "," + sr.ToString());
                    sigs.updateCurrentSignal(callsign, freq, sr);
                }
            }
        }

        private void SpectrumTuneTimer_Tick(object sender, EventArgs e)
        {
            int mode = 0;
            float spectrum_w = spectrum.Width;
            float spectrum_wScale = spectrum_w / 922;

            if (autoTimedToolStripMenuItem.Checked)
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
            if (ret.Item1.frequency > 0) //above 0 is a change in signal
            {
                Thread.Sleep(100);
                selectSignal(Convert.ToInt32(ret.Item1.fft_centre * spectrum_wScale), 0);
                sigs.set_tuned(ret.Item1, 0);
                rx_blocks[0, 0] = Convert.ToInt16(ret.Item1.fft_centre);
                rx_blocks[0, 1] = Convert.ToInt16(ret.Item1.fft_stop - ret.Item1.fft_start);
            }
        }

        /*
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
        */

        private void toggleFullscreenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*
            try
            {
                toggleFullScreen();
            }
            catch (Exception Ex)
            {
    
            }
            */
        }


        private void manageStoredFrequenciesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrequencyManagerForm freqForm = new FrequencyManagerForm(stored_frequencies);
            freqForm.ShowDialog();

            save_stored_frequencies();
            rebuild_stored_frequencies();
        }

        private void lblChatSigReport_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            float freq = mt.current_frequency_1 + mt.current_offset_A;
            freq = freq / 1000;

            //string signalReport = "SigReport: " + lblServiceName.Text.ToString() + "/" + lblServiceProvider.Text.ToString() + " - " + lbldbMargin.Text.ToString() + " (" + lblMer.Text.ToString() + ") - " + lblSR.Text.ToString() + "" + " - " + (freq).ToString() + " ";
            string signalReport = setting_sigreport_template.ToString();

            // SigReport: {SN}/{SP} - {DBM} - ({MER}) - {SR} - {FREQ}

            signalReport = signalReport.Replace("{SN}", lblServiceName.Text);
            signalReport = signalReport.Replace("{SP}", lblServiceProvider.Text);
            signalReport = signalReport.Replace("{DBM}", lbldbMargin.Text);
            signalReport = signalReport.Replace("{MER}", lblMer.Text);
            signalReport = signalReport.Replace("{SR}", lblSR.Text + "");
            signalReport = signalReport.Replace("{FREQ}", freq.ToString() + "");

            chatForm.txtMessage.Text = signalReport;

            Clipboard.SetText(signalReport);
        }

        private void manualToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SpectrumTuneTimer.Enabled = false;
            manualToolStripMenuItem.Checked = true;
            autoHoldToolStripMenuItem.Checked = false;
            autoTimedToolStripMenuItem.Checked = false;
        }

        private void autoTimedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SpectrumTuneTimer.Enabled = true;
            manualToolStripMenuItem.Checked = false;
            autoHoldToolStripMenuItem.Checked = false;
            autoTimedToolStripMenuItem.Checked = true;
        }

        private void autoHoldToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SpectrumTuneTimer.Enabled = true;
            manualToolStripMenuItem.Checked = false;
            autoHoldToolStripMenuItem.Checked = true;
            autoTimedToolStripMenuItem.Checked = false;
        }

        private void btnVid1Mute_Click(object sender, EventArgs e)
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

        private void btnVid1Snapshot_Click(object sender, EventArgs e)
        {
            TakeSnapshot(1);
        }

        private void btnVid2Snapshot_Click(object sender, EventArgs e)
        {
            TakeSnapshot(2);
        }

        private void btnVid2Mute_Click(object sender, EventArgs e)
        {
            if (rxVolume2 == 0)
            {
                rxVolume2 = beforeMute2;
            }
            else
            {
                beforeMute2 = rxVolume2;
                rxVolume2 = 0;
            }

            trackVolume2.Value = rxVolume2;
        }

        private void trackVolume2_ValueChanged(object sender, EventArgs e)
        {
            rxVolume2 = Convert.ToByte(trackVolume2.Value);

            if (media_player_2 != null)
            {
                media_player_2.SetVolume(rxVolume2);
            }

            lblVolume2.Text = rxVolume2.ToString() + " %";
        }

        private void lblAdjust1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            uint carrier_offset = 0;

            if (UInt32.TryParse(lblFreqCar.Text, out carrier_offset))
            {
                carrier_offset = carrier_offset / 1000;
                mt.change_frequency(1, (mt.current_frequency_1 + carrier_offset), mt.current_sr_1,
                    mt.current_enable_lnb_supply, mt.current_enable_horiz_supply, mt.current_rf_input_1,
                    mt.current_tone_22kHz_P1);
            }
        }

        private void lblAdjust2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            uint carrier_offset = 0;

            if (UInt32.TryParse(lblFreqCar2.Text, out carrier_offset))
            {
                carrier_offset = carrier_offset / 1000;
                mt.change_frequency(2, (mt.current_frequency_2 + carrier_offset), mt.current_sr_2,
                    mt.current_enable_lnb_supply, mt.current_enable_horiz_supply, mt.current_rf_input_2,
                    mt.current_tone_22kHz_P1);
            }
        }

        private void btnVid1Record_Click(object sender, EventArgs e)
        {
            if (ts_recorder1 != null)
            {
                if (ts_recorder1.record)
                    ts_recorder1.record = false;
                else
                    ts_recorder1.record = true;
            }
        }

        private void recordAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            recordAllToolStripMenuItem.Checked = !recordAllToolStripMenuItem.Checked;
        }

        private void enableUDPOutputToolStripMenuItem_Click(object sender, EventArgs e)
        {
            enableUDPOutputToolStripMenuItem.Checked = !enableUDPOutputToolStripMenuItem.Checked;

            if (ts_udp1 != null)
            {
                if (enableUDPOutputToolStripMenuItem.Checked)
                {
                    ts_udp1.stream = true;
                }
                else
                {
                    ts_udp1.stream = false;
                }
            }
        }

        private void btnTuner_Click(object sender, EventArgs e)
        {
            tuner1ControlForm.Show();
            tuner1ControlForm.Focus();
        }

        private void btnTuner2_Click(object sender, EventArgs e)
        {
            tuner2ControlForm.Show();
            tuner2ControlForm.Focus();
        }

        private void lblServiceName2_TextChanged(object sender, EventArgs e)
        {
            if (setting_enable_spectrum)
            {
                // we have decoded a callsign
                string callsign = lblServiceName2.Text;
                int offset = 0;

                //Int32.TryParse(current, out offset);

                if (mt.current_rf_input_2 == nim.NIM_INPUT_TOP)
                    offset = mt.current_offset_A;
                else
                    offset = mt.current_offset_B;

                if (callsign.Length > 0)
                {
                    double freq = mt.current_frequency_2 + offset;
                    freq = freq / 1000;
                    float sr = mt.current_sr_2;

                    debug("New Callsign: " + callsign + "," + freq.ToString() + "," + sr.ToString());
                    sigs.updateCurrentSignal(callsign, freq, sr);
                }
            }
        }

        private void offToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!mt.hardware_connected)
                return;

            mt.ftdi_hw.ftdi_set_polarization_supply(0, false, false);

            offToolStripMenuItem.Checked = true;
            vertical13VToolStripMenuItem.Checked = false;
            horizontal18VToolStripMenuItem.Checked = false;
        }

        private void vertical13VToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!mt.hardware_connected)
                return;

            mt.ftdi_hw.ftdi_set_polarization_supply(0, true, false);

            offToolStripMenuItem.Checked = false;
            vertical13VToolStripMenuItem.Checked = true;
            horizontal18VToolStripMenuItem.Checked = false;
        }

        private void horizontal18VToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!mt.hardware_connected)
                return;

            mt.ftdi_hw.ftdi_set_polarization_supply(0, true, true);

            offToolStripMenuItem.Checked = false;
            vertical13VToolStripMenuItem.Checked = false;
            horizontal18VToolStripMenuItem.Checked = true;
        }

        private void kHzToolStripMenuItem_Click(object sender, EventArgs e)
        {
            kHzToolStripMenuItem.Checked = !kHzToolStripMenuItem.Checked;
            mt.change_frequency(1, mt.current_frequency_1, mt.current_sr_1, mt.current_enable_lnb_supply,
                mt.current_enable_horiz_supply, mt.current_rf_input_1, kHzToolStripMenuItem.Checked);
        }

        private void enableUDPOutputToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            enableUDPOutputToolStripMenuItem1.Checked = !enableUDPOutputToolStripMenuItem1.Checked;

            if (ts_udp2 != null)
            {
                if (enableUDPOutputToolStripMenuItem1.Checked)
                {
                    ts_udp2.stream = true;
                }
                else
                {
                    ts_udp2.stream = false;
                }
            }
        }

        private void addingA2ndTransportToBATCMinitiounerV2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.zr6tg.co.za/adding-2nd-transport-to-batc-minitiouner-v2/");
        }

        private void btnVidRecord2_Click(object sender, EventArgs e)
        {
            if (ts_recorder2 != null)
            {
                if (ts_recorder2.record)
                    ts_recorder2.record = false;
                else
                    ts_recorder2.record = true;
            }
        }

        private void spectrum_MouseLeave(object sender, EventArgs e)
        {
            spectrumTunerHighlight = 0;
        }

        private void checkDisableVideo2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkDisableVideo2.Checked)
            {
                stop_video2();

                lblVideoCodecTitle.Enabled = false;
                lblVideoResTitle.Enabled = false;
                lblAudioCodecTitle.Enabled = false;
                lblAudioRateTitle.Enabled = false;
                lblServiceNameTitle2.Enabled = false;
                lblServiceProviderTitle2.Enabled = false;
                lblNullPacketsTitle2.Enabled = false;
                lblNullPackets2.Enabled = false;
                nullPacketsBar2.Enabled = false;

                lblVideoCodec2.Text = "";
                lblVideoResolution2.Text = "";
                lblAudioRate2.Text = "";
                lblAudioCodec2.Text = "";

                lblServiceName2.Text = "";
                lblServiceProvider2.Text = "";
                lblNullPackets2.Text = "";
                nullPacketsBar.Value = 0;

                if (setting_windowed_mediaPlayer2 == false)
                {
                    videoPlayersSplitter.Panel2.Hide();
                    videoPlayersSplitter.Panel2Collapsed = true;
                }
            }
            else
            {
                lblVideoCodecTitle.Enabled = true;
                lblVideoResTitle.Enabled = true;
                lblAudioCodecTitle.Enabled = true;
                lblAudioRateTitle.Enabled = true;
                lblServiceNameTitle2.Enabled = true;
                lblServiceProviderTitle2.Enabled = true;
                lblNullPacketsTitle2.Enabled = true;
                lblNullPackets2.Enabled = true;
                nullPacketsBar2.Enabled = true;

                if (setting_windowed_mediaPlayer2 == false)
                {
                    videoPlayersSplitter.Panel2.Show();
                    videoPlayersSplitter.Panel2Collapsed = false;
                }
            }
        }

        private void toolStripMenuItem7_Click(object sender, EventArgs e)
        {
            // detect ftdi devices
            HardwareInfoForm hwForm = new HardwareInfoForm(mt.ftdi_hw);
            hwForm.ShowDialog();
        }

        private void manageExternalToolsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExternalToolsManager etManager = new ExternalToolsManager(external_tools);
            etManager.ShowDialog();

            save_external_tools();
            rebuild_external_tools();
        }

        /*
        private void btnNotifyTX_Click(object sender, EventArgs e)
        {
            tx_udp_send(0,0);
        }
    
        private void tx_udp_send(int freq, int symbol_rate)
        {
            string data = "Freq=438000 Srate=1500";
            byte[] outStream = Encoding.ASCII.GetBytes(data);
            tx_end_point = new System.Net.IPEndPoint(IPAddress.Parse("127.0.0.1"), 9920);
    
            try
            {
                tx_client.Client.SendTo(outStream, tx_end_point);
            }
            catch( Exception ex) 
            { 
                Console.WriteLine("Error sending command to TX UDP: " + ex.Message );
            }
        }
                */
        private class ToolStripRenderer : ToolStripProfessionalRenderer
        {
            public ToolStripRenderer() : base(new MyColors())
            {
            }
        }

        private class MyColors : ProfessionalColorTable
        {
            private Color _darkTheme = Color.FromArgb(63, 70, 76);

            public override Color MenuItemSelected
            {
                get { return _darkTheme; }
            }

            public override Color MenuItemSelectedGradientBegin
            {
                get { return _darkTheme; }
            }

            public override Color MenuItemSelectedGradientEnd
            {
                get { return _darkTheme; }
            }

            public override Color MenuItemPressedGradientEnd
            {
                get { return _darkTheme; }
            }

            public override Color MenuItemPressedGradientBegin
            {
                get { return _darkTheme; }
            }

            public override Color MenuStripGradientBegin
            {
                get { return _darkTheme; }
            }

            public override Color MenuStripGradientEnd
            {
                get { return _darkTheme; }
            }
        }

        private void OTMenuItemsColoring()
        {
            var allMenuStripItems = GetAllMenuStripItems(menuStrip1);

            foreach (var toolStripMenuItem in allMenuStripItems)
            {
                toolStripMenuItem.BackColor = _darkTheme;
                toolStripMenuItem.ForeColor = _lightTheme;
            }

            var contextMenuStripItems = GetAllContextMenuStripItems(contextSpectrumMenu);
            foreach (var toolStripMenuItem in contextMenuStripItems)
            {
                toolStripMenuItem.BackColor = _darkTheme;
                toolStripMenuItem.ForeColor = _lightTheme;
            }
        }

        //Extract all menu strip items
        private List<ToolStripMenuItem> GetAllMenuStripItems(MenuStrip mnuStrip)
        {
            toolSripItems = new List<ToolStripMenuItem>();
            foreach (ToolStripMenuItem toolSripItem in mnuStrip.Items)
            {
                GetAllSubMenuStripItems(toolSripItem);
            }
            return toolSripItems;
        }
        //Extract all context menu strip items
        private List<ToolStripMenuItem> GetAllContextMenuStripItems(ContextMenuStrip mnuStrip)
        {
            toolSripItems = new List<ToolStripMenuItem>();
            foreach (ToolStripMenuItem toolSripItem in mnuStrip.Items)
            {
                GetAllSubMenuStripItems(toolSripItem);
            }
            return toolSripItems;
        }

        //This method is called recursively inside to loop through all menu items
        private void GetAllSubMenuStripItems(ToolStripMenuItem mnuItem)
        {
            toolSripItems.Add(mnuItem);

            // if sub menu contain child dropdown items
            if (mnuItem.HasDropDownItems)
            {
                foreach (ToolStripItem toolSripItem in mnuItem.DropDownItems)
                {
                    if (toolSripItem is ToolStripMenuItem)
                    {
                        //call the method recursively to extract further.
                        GetAllSubMenuStripItems((ToolStripMenuItem)toolSripItem);
                    }
                }
            }
        }
    }
}