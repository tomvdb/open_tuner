using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;
using System.Diagnostics;

using opentuner.MediaSources;
using opentuner.MediaSources.Minitiouner;

using opentuner.MediaPlayers;
using opentuner.MediaPlayers.MPV;
using opentuner.MediaPlayers.FFMPEG;
using opentuner.MediaPlayers.VLC;

using opentuner.Utilities;
using opentuner.Transmit;
using opentuner.ExtraFeatures.BATCSpectrum;
using opentuner.ExtraFeatures.MqttClient;
using opentuner.MediaSources.Longmynd;

namespace opentuner
{
    delegate void updateNimStatusGuiDelegate(MainForm gui, TunerStatus new_status);
    delegate void updateTSStatusGuiDelegate(int device, MainForm gui, TSStatus new_status);
    delegate void updateMediaStatusGuiDelegate(int tuner, MainForm gui, MediaStatus new_status);
    delegate void UpdateLBDelegate(ListBox LB, Object obj);
    delegate void updateRecordingStatusDelegate(MainForm gui, bool recording_status, string id);

    public partial class MainForm : Form
    {
        // extras
        OTMqttClient mqtt_client;
        F5OEOPlutoControl pluto_client;
        BATCSpectrum batc_spectrum;

        private static List<OTMediaPlayer> _mediaPlayers;
        private static List<OTSource> _availableSources = new List<OTSource>();
        private static OTSource videoSource;

        // udp listener
        UdpListener ExternalQuickTuneListener_1 = new UdpListener(6789);
        UdpListener ExternalQuickTuneListener_2 = new UdpListener(6790);


        Thread ts_recorder_1_t = null;
        Thread ts_recorder_2_t = null;
        Thread ts_udp_t1 = null;
        Thread ts_udp_t2 = null;

        TSRecorderThread ts_recorder1;
        TSRecorderThread ts_recorder2;
        TSUDPThread ts_udp1;
        TSUDPThread ts_udp2;

        // settings values
        string setting_snapshot_path = "";
        bool setting_enable_spectrum = true;
        byte setting_default_lnb_supply = 0;    // 0 - off, 1 - vert, 2 - horiz
        int setting_default_lo_value_1 = 0;
        int setting_default_lo_value_2 = 0;
        int setting_default_volume = 100;
        int setting_default_volume2 = 100;
        int setting_language = 0;
        bool setting_enable_chatform = false;
        bool setting_auto_connect = false;

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

        int setting_default_hardware = 0;

        // mqtt settings
        bool setting_enable_mqtt = false;
        string setting_mqtt_broker_host = "127.0.0.1";
        int setting_mqtt_broker_port = 1883;
        string setting_mqtt_parent_topic = "";

        // f5oeoe firmware pluto
        bool setting_enable_pluto = false;

        // custom forms
        private wbchat chatForm;
        private tunerControlForm tuner1ControlForm;
        private tunerControlForm tuner2ControlForm;
        private VideoViewForm mediaPlayer1Window;
        private VideoViewForm mediaPlayer2Window;

        List<StoredFrequency> stored_frequencies = new List<StoredFrequency>();
        List<ExternalTool> external_tools = new List<ExternalTool>();

        public MainForm()
        {
            ThreadPool.GetMinThreads(out int workers, out int ports);
            ThreadPool.SetMinThreads(workers + 6, ports + 6);

            InitializeComponent();

            // available sources
            _availableSources.Add(new MinitiounerSource());
            _availableSources.Add(new LongmyndSource());

            comboAvailableSources.Items.Clear();

            for (int c = 0; c < _availableSources.Count; c++)
            {
                comboAvailableSources.Items.Add(_availableSources[c].GetName());
            }

            comboAvailableSources.SelectedIndex = 0;
            sourceInfo.Text = _availableSources[0].GetDescription();

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

            // test
            //SoftBlink(lblrecordIndication1, Color.FromArgb(255, 255, 255), Color.Red, 2000, false);
            //SoftBlink(lblRecordIndication2, Color.FromArgb(255, 255, 255), Color.Red, 2000, false);

            // udp listeners
            ExternalQuickTuneListener_1.DataReceived += ExternalQuickTuneListener_1_DataReceived;
            ExternalQuickTuneListener_2.DataReceived += ExternalQuickTuneListener_2_DataReceived;

            ExternalQuickTuneListener_1.StartListening();
            ExternalQuickTuneListener_2.StartListening();
        }

        /// <summary>
        /// Connect to Media Source and configure Media Players + extra's based on Media Source initialization.
        /// </summary>
        /// <param name="MediaSource"></param>
        private bool SourceConnect(OTSource MediaSource)
        {
            videoSource = MediaSource;

            int video_players_required = videoSource.Initialize(ChangeVideo, PropertiesPage);

            if (video_players_required < 0)
            {
                MessageBox.Show("Error Connecting MediaSource: " + videoSource.GetName());
                return false;
            }

            this.Text = this.Text += " - " + videoSource.GetDeviceName();

            // TS recorder Thread
            ts_recorder1 = new TSRecorderThread(setting_snapshot_path, "t1");
            videoSource.RegisterTSConsumer(0, ts_recorder1.ts_data_queue);
            ts_recorder1.onRecordStatusChange += Ts_recorder_onRecordStatusChange;
            ts_recorder_1_t = new Thread(ts_recorder1.worker_thread);
            ts_recorder_1_t.Start();

            // TS udp thread - tuner 1
            ts_udp1 = new TSUDPThread(setting_udp_address1, setting_udp_port1);
            videoSource.RegisterTSConsumer(0, ts_udp1.ts_data_queue);
            ts_udp_t1 = new Thread(ts_udp1.worker_thread);
            ts_udp_t1.Start();

            if (videoSource.GetVideoSourceCount() == 2)
            {
                // TS udp thread - tuner 2
                ts_udp2 = new TSUDPThread(setting_udp_address2, setting_udp_port2);
                videoSource.RegisterTSConsumer(1, ts_udp2.ts_data_queue);
                ts_udp_t2 = new Thread(ts_udp2.worker_thread);
                ts_udp_t2.Start();

                // TS recorder Thread
                ts_recorder2 = new TSRecorderThread(setting_snapshot_path, "t2");
                videoSource.RegisterTSConsumer(1, ts_recorder2.ts_data_queue);
                ts_recorder2.onRecordStatusChange += Ts_recorder_onRecordStatusChange;
                ts_recorder_2_t = new Thread(ts_recorder2.worker_thread);
                ts_recorder_2_t.Start();

            }

            // preferred player to use for each video view
            // 0 = vlc, 1 = ffmpeg, 2 = mpv
            int[] playerPreferences = new int[] { 0, 1, 1, 1 };

            _mediaPlayers = ConfigureMediaPlayers(videoSource.GetVideoSourceCount(), playerPreferences);
            videoSource.ConfigureVideoPlayers(_mediaPlayers);

            // update gui
            SourcePage.Hide();
            tabControl1.TabPages.Remove(SourcePage);

            return true;
        }

        private async void SoftBlink(Control ctrl, Color c1, Color c2, short CycleTime_ms, bool BkClr)
        {
            var sw = new Stopwatch(); sw.Start();
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
                if (BkClr) ctrl.BackColor = clr; else ctrl.ForeColor = clr;
            }
        }

        /*
        public static void updateRecordingStatus(MainForm gui, bool recording_status, string id)
        {

                if (gui == null)
                    return;

                if (gui.InvokeRequired)
                {
                    updateRecordingStatusDelegate ulb = new updateRecordingStatusDelegate(updateRecordingStatus);

                    if (gui != null)
                        gui?.Invoke(ulb, new object[] { gui, recording_status, id });
                }
                else
                {
                    if (id == "t1")
                        gui.prop_isRecording1 = recording_status;
                    else
                        gui.prop_isRecording2 = recording_status;
                }
        }
        */


        public static void UpdateLB(ListBox LB, Object obj)
        {

            if (LB == null)
                return;

            if (LB.InvokeRequired)
            {
                UpdateLBDelegate ulb = new UpdateLBDelegate(UpdateLB);
                if (LB != null)
                {
                    Console.WriteLine("Invoking");
                    LB?.Invoke(ulb, new object[] { LB, obj });
                }
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





        private void ExternalQuickTuneListener_2_DataReceived(object sender, Utilities.DataReceivedEventArgs e)
        {
            try
            {
                Console.WriteLine("UDP Received (2): " + e.Message);

                string[] properties = e.Message.Split(',');

                uint freq = 0;
                uint offset = 0;
                uint sr = 0;

                uint.TryParse(properties[1].Substring(5), out freq);
                uint.TryParse(properties[2].Substring(7), out offset);
                uint.TryParse(properties[4].Substring(6), out sr);

                Console.WriteLine("New Freq Request (1) = " + (freq - offset).ToString() + "," + sr.ToString() + " ks");

                //change_frequency_with_lo(1, freq, offset, sr);
            }
            catch (Exception Ex)
            {

            }
        }

        private void ExternalQuickTuneListener_1_DataReceived(object sender, Utilities.DataReceivedEventArgs e)
        {
            try
            {
                Console.WriteLine("UDP Received (1): " + e.Message);

                string[] properties = e.Message.Split(',');

                uint freq = 0;
                uint offset = 0;
                uint sr = 0;

                uint.TryParse(properties[1].Substring(5), out freq);
                uint.TryParse(properties[2].Substring(7), out offset);
                uint.TryParse(properties[4].Substring(6), out sr);

                Console.WriteLine("New Freq Request (2) = " + (freq - offset).ToString() + "," + sr.ToString() + " ks");

                //change_frequency_with_lo(2, freq, offset, sr);
            }
            catch( Exception Ex )
            {

            }

        }

        public void start_video(int video_number)
        {
            if (_mediaPlayers == null)
                return;

            if (video_number < _mediaPlayers.Count)
            {
                videoSource.StartStreaming(video_number);
                _mediaPlayers[video_number].Play();
            }
        }

        public void stop_video(int video_number)
        {
            if (_mediaPlayers == null)
                return;

            if (video_number < _mediaPlayers.Count)
            {
                videoSource.StopStreaming(video_number);
                _mediaPlayers[video_number].Stop();
            }
        }

        private void ChangeVideo(int video_number, bool start)
        {
            Console.WriteLine("Change Video " + video_number.ToString());

            if (start)
                start_video(video_number-1);
            else 
                stop_video(video_number-1);
        }

        /*
        public void start_video1()
        {
            Console.WriteLine("Main: Starting Media Player 1");

            if (media_player_1 != null)
                media_player_1.Play();
        }

        public void start_video2()
        {
           if (media_player_2 != null)
                    media_player_2.Play();
        }


        public void stop_video1()
        {
            Console.WriteLine("Main: Stopping Media Player 1");

            if (media_player_1 != null)
                media_player_1.Stop();


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

            if (media_player_2 != null)
                media_player_2.Stop();

            if (ts_recorder2 != null)
                ts_recorder2.record = false;

            if (ts_udp2 != null)
                ts_udp2.stream = false;
        }

        */

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Console.WriteLine("Exiting...");
            Console.WriteLine("* Saving Settings");

            // save chat location settings
            if (setting_enable_chatform && chatForm != null)
            {
                Properties.Settings.Default.wbchat_height = chatForm.Size.Width;
                Properties.Settings.Default.wbchat_width = chatForm.Size.Height;
            }

            // save current volume as default volume
            //Properties.Settings.Default.default_volume = trackVolume.Value;
            //Properties.Settings.Default.default_volume2 = trackVolume2.Value;

            // save current windows properties
            Properties.Settings.Default.window_width = this.Width;
            Properties.Settings.Default.window_height = this.Height;
            Properties.Settings.Default.window_x = this.Left;
            Properties.Settings.Default.window_y = this.Top;
            Properties.Settings.Default.main_splitter_pos = splitContainer1.SplitterDistance;

            Properties.Settings.Default.Save();

            try
            {
                if (mqtt_client !=  null)
                {
                    mqtt_client.Disconnect();
                }

                if (batc_spectrum != null)
                    batc_spectrum.Close();

                Console.WriteLine("* Stopping Playing Video");

                if (videoSource != null)
                {
                    ChangeVideo(1, false);
                    if (videoSource.GetVideoSourceCount() == 2)
                        ChangeVideo(2, false);
                }

                Console.WriteLine("* Closing Extra TS Threads");

                // close threads
                if (ts_recorder_1_t != null)
                    ts_recorder_1_t.Abort();
                if (ts_recorder_2_t != null)
                    ts_recorder_2_t.Abort();
                if (ts_udp_t1 != null)
                    ts_udp_t1.Abort();
                if (ts_udp_t2 != null)
                    ts_udp_t2.Abort();

                // close available media sources
                for (int c = 0; c < _availableSources.Count; c++)
                {
                    _availableSources[c].Close();
                }

                // close forms
                if (chatForm != null)
                    chatForm.Close();

                if (tuner1ControlForm != null)
                    tuner1ControlForm.Close();
                if (tuner2ControlForm != null)
                    tuner2ControlForm.Close();

                // close udp
                ExternalQuickTuneListener_1.StopListening();
                ExternalQuickTuneListener_2.StopListening();
            }
            catch ( Exception Ex)
            {
                // we are closing, we don't really care about exceptions at this point
                Console.WriteLine("Closing Exception: " + Ex.Message);
            }

            Console.WriteLine("Done");

        }

        private void btnConnectTuner_Click(object sender, EventArgs e)
        {
            /*
            int hardware_source = setting_default_hardware - 1;

            // no default, ask user
            if (hardware_source == -1)
            {
                var hardwareInterfaceSelection = new ChooseHardwareInterfaceForm();

                if (hardwareInterfaceSelection.ShowDialog() == DialogResult.OK)
                {
                    hardware_source = hardwareInterfaceSelection.comboHardwareSelect.SelectedIndex;
                }
            }

            videoSource.SelectHardwareInterface(hardware_source);
            
            Console.WriteLine("Main: Starting Source");
            */

            int video_players_required = videoSource.Initialize(ChangeVideo, PropertiesPage);

            if (video_players_required < 0)
            {
                Console.WriteLine("Main: Error initializing Source");
                return;
            }

            this.Text = this.Text += " - " + videoSource.GetDeviceName();

            // TS recorder Thread
            ts_recorder1 = new TSRecorderThread(setting_snapshot_path, "t1");
            videoSource.RegisterTSConsumer(0, ts_recorder1.ts_data_queue);
            ts_recorder1.onRecordStatusChange += Ts_recorder_onRecordStatusChange;
            ts_recorder_1_t = new Thread(ts_recorder1.worker_thread);
            ts_recorder_1_t.Start();

            // TS udp thread - tuner 1
            ts_udp1 = new TSUDPThread(setting_udp_address1, setting_udp_port1);
            videoSource.RegisterTSConsumer(0, ts_udp1.ts_data_queue);
            ts_udp_t1 = new Thread(ts_udp1.worker_thread);
            ts_udp_t1.Start();

            if (videoSource.GetVideoSourceCount() == 2)
            {
                // TS udp thread - tuner 2
                ts_udp2 = new TSUDPThread( setting_udp_address2, setting_udp_port2);
                videoSource.RegisterTSConsumer(1, ts_udp2.ts_data_queue);
                ts_udp_t2 = new Thread(ts_udp2.worker_thread);
                ts_udp_t2.Start();

                // TS recorder Thread
                ts_recorder2 = new TSRecorderThread(setting_snapshot_path, "t2");
                videoSource.RegisterTSConsumer(1, ts_recorder2.ts_data_queue);
                ts_recorder2.onRecordStatusChange += Ts_recorder_onRecordStatusChange;
                ts_recorder_2_t = new Thread(ts_recorder2.worker_thread);
                ts_recorder_2_t.Start();

            }

            // preferred player to use for each video view
            // 0 = vlc, 1 = ffmpeg, 2 = mpv
            int[] playerPreferences = new int[] { 0, 1, 1, 1 };

            _mediaPlayers = ConfigureMediaPlayers(videoSource.GetVideoSourceCount(), playerPreferences);
            videoSource.ConfigureVideoPlayers(_mediaPlayers);

            //menuConnect.Enabled = false;
        }

        /*
        private void MediaPlayer_Vout2(object sender, MediaStatus media_status)
        {
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
        */

        private void Ts_recorder_onRecordStatusChange(object sender, bool e)
        {
            TSRecorderThread whichRecorder = (TSRecorderThread)sender;
            //updateRecordingStatus(this, e, whichRecorder.id);
        }

        /*
        private void MediaPlayer_Vout(object sender, MediaStatus media_status)
        {
            if (media_player_1 != null)
                media_player_1.SetVolume(rxVolume);

            if (recordAllToolStripMenuItem.Checked)
            {
                if (ts_recorder1 != null)
                    ts_recorder1.record = true;  // recording will automatically stop when lock is lost
            }

            if (enableUDPOutputToolStripMenuItem.Checked)
            {
                if (ts_udp1 != null)
                {
                    ts_udp1.stream = true;
                }
            }


        }
        */



        
        private void load_settings()
        {
            // warning: don't use the debug function in this function as it gets called before components are initialized

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

            setting_default_hardware = Properties.Settings.Default.default_source_hardware;

            // mqtt settings
            setting_enable_mqtt = Properties.Settings.Default.enable_mqtt;
            setting_mqtt_broker_host = Properties.Settings.Default.mqtt_broker_host;
            setting_mqtt_broker_port = Properties.Settings.Default.mqtt_broker_port;
            setting_mqtt_parent_topic = Properties.Settings.Default.mqtt_topic_parent;

            // pluto settings
            setting_enable_pluto = Properties.Settings.Default.enable_pluto;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (setting_snapshot_path.Length == 0)
                setting_snapshot_path = AppDomain.CurrentDomain.BaseDirectory;


            //debug("Settings: Restore Last Volume: " + setting_default_volume.ToString() + "%");
            //trackVolume.Value = setting_default_volume;
            //debug("Settings: Restore Last Volume2: " + setting_default_volume2.ToString() + "%");
            //trackVolume2.Value = setting_default_volume2;
            //debug("Settings: Default Offset A: " + setting_default_lo_value_1.ToString());
            //videoSource.current_offset_A = setting_default_lo_value_1;
            //debug("Settings: Default Offset B: " + setting_default_lo_value_2.ToString());
            //videoSource.current_offset_B = setting_default_lo_value_2;


            debug("Settings: Snapshot Path: " + setting_snapshot_path);

            offToolStripMenuItem.Checked = false;
            vertical13VToolStripMenuItem.Checked = false;
            horizontal18VToolStripMenuItem.Checked = false;

            /*
            switch (setting_default_lnb_supply)
            {
                case 0:
                    videoSource.current_enable_lnb_supply = false;
                    videoSource.current_enable_horiz_supply = false;
                    offToolStripMenuItem.Checked = true;
                    break;
                case 1:
                    videoSource.current_enable_lnb_supply = true;
                    videoSource.current_enable_horiz_supply = false;
                    vertical13VToolStripMenuItem.Checked = true;
                    break;
                case 2:
                    videoSource.current_enable_lnb_supply = true;
                    videoSource.current_enable_horiz_supply = true;
                    horizontal18VToolStripMenuItem.Checked = true;
                    break;
            }

            debug("Settings: Enable LNB Supply: " + videoSource.current_enable_lnb_supply.ToString());

            if (videoSource.current_enable_lnb_supply)
            {
                debug("Settings: Enable Vert Supply: " + (!videoSource.current_enable_horiz_supply).ToString());
                debug("Settings: Enable Horiz Supply: " + videoSource.current_enable_horiz_supply.ToString());
            }
            */

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

                /*
                if (arg.StartsWith("MANUAL="))
                {
                    string param = arg.Substring(7);
                    Console.WriteLine("Manual Device Specifed! : " + param);

                    manual_serial_devices = param.Split(',');

                    for (int c = 0; c < manual_serial_devices.Length; c++)
                    {
                        switch(c)
                        {
                            case 0: Console.WriteLine("I2C: " + manual_serial_devices[c].ToString()); break;
                            case 1: Console.WriteLine("Tuner 1 TS: " + manual_serial_devices[c].ToString()); break;
                            case 2: Console.WriteLine("Tuner 2 TS: " + manual_serial_devices[c].ToString()); break;
                        }
                    }

                    setting_autodetect = false;
                }
                */


            }

            if (setting_enable_chatform)
            {
                qO100WidebandChatToolStripMenuItem.Visible = true;
                chatForm = new wbchat(setting_chat_font_size);

                if (setting_chat_width > -1 && setting_chat_height > -1)
                {
                    chatForm.Size = new Size(setting_chat_height, setting_chat_width);
                }
            }
            else
            {
                qO100WidebandChatToolStripMenuItem.Visible = false;
            }



            Console.WriteLine("Restoring Window Positions:");
            Console.WriteLine(" Size: (" + setting_window_height.ToString() + "," + setting_window_width.ToString() + ")");
            Console.WriteLine(" Position: (" + setting_window_x.ToString() + "," + setting_window_y.ToString() + ")");

            this.Height = setting_window_height;
            this.Width = setting_window_width;

            this.Left = setting_window_x;
            this.Top = setting_window_y;

            splitContainer1.SplitterDistance = setting_main_splitter_position;

            load_stored_frequencies();
            rebuild_stored_frequencies();

            load_external_tools();
            rebuild_external_tools();

            // tuner control windows
            /*
            TunerChangeCallback tuner1Callback = new TunerChangeCallback(tuner1_change_callback);
            TunerChangeCallback tuner2Callback = new TunerChangeCallback(tuner2_change_callback);

            tuner1ControlForm = new tunerControlForm(tuner1Callback);
            tuner2ControlForm = new tunerControlForm(tuner2Callback);

            tuner1ControlForm.Text = "Tuner 1";
            tuner2ControlForm.Text = "Tuner 2";

            tuner1ControlForm.set_offset(videoSource.current_offset_A, videoSource.current_offset_B);
            tuner2ControlForm.set_offset(videoSource.current_offset_A, videoSource.current_offset_B);
            */

            Console.WriteLine("Load Done");

            // mqtt client
            if (setting_enable_mqtt)
            {
                mqtt_client = new OTMqttClient(setting_mqtt_broker_host, setting_mqtt_broker_port, setting_mqtt_parent_topic);
                mqtt_client.OnMqttMessageReceived += Mqtt_client_OnMqttMessageReceived;

                // pluto - requires mqtt
                if (setting_enable_pluto)
                {
                    pluto_client = new F5OEOPlutoControl(mqtt_client);
                    plutoToolStripMenuItem.Visible = true;
                }
            }

            

            // this needs to go last
            if (setting_auto_connect)
            {
                //btnConnectTuner_Click(this, e);
            }
        }

        private void Batc_spectrum_OnSignalSelected(int Receiver, uint Freq, uint SymbolRate)
        {
            videoSource.SetFrequency(Receiver, Freq, SymbolRate, true);
        }

        private void Mqtt_client_OnMqttMessageReceived(MqttMessage Message)
        {
            //Console.WriteLine("Main: " + Message.ToString());
        }

        private void rebuild_external_tools()
        {
            externalToolsToolStripMenuItem1.DropDownItems.Clear();

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
                    if (external_tools[tag].EnableUDP1 )
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

                    System.Diagnostics.Process.Start(external_tools[tag].ToolPath, parameters);
                }
                catch( Exception Ex) 
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
                ToolStripMenuItem sf_menu = new ToolStripMenuItem(stored_frequencies[c].Name + " (" + stored_frequencies[c].Frequency + ")( Tuner " + (stored_frequencies[c].DefaultTuner + 1).ToString() + ")");
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
            //if (videoSource != null)
            //    videoSource.change_frequency(Convert.ToByte(sf.DefaultTuner + 1), sf.Frequency - sf.Offset, sf.SymbolRate, videoSource.current_enable_lnb_supply, videoSource.current_enable_horiz_supply, (uint)sf.RFInput, videoSource.current_tone_22kHz_P1);
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
            catch(Exception Ex) 
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
            catch(Exception Ex)
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
            catch(Exception Ex)
            {
                Console.WriteLine("Error writing frequencies file: " + Ex.Message);
            }
        }





        /*
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
        */

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

            settings_form.comboDefaultHardware.SelectedIndex = setting_default_hardware;

            // mqtt
            settings_form.checkEnableMqtt.Checked = setting_enable_mqtt;
            settings_form.txtBrokerHost.Text = setting_mqtt_broker_host;
            settings_form.numBrokerPort.Value = setting_mqtt_broker_port;
            settings_form.txtParentTopic.Text = setting_mqtt_parent_topic;

            // pluto
            settings_form.checkPlutoControl.Checked = setting_enable_pluto;

            for ( int c = 0; c < stored_frequencies.Count; c++)
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


            if ( settings_form.ShowDialog() == DialogResult.OK )
            {
                //Properties.Settings.Default.wbchat_font = settings_form.currentChatFont;
                Properties.Settings.Default.wbchat_font_size = Convert.ToInt32(settings_form.numChatFontSize.Value);

                Properties.Settings.Default.default_lnb_supply = Convert.ToByte(settings_form.comboDefaultLNB.SelectedIndex);
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

                Properties.Settings.Default.default_source_hardware = settings_form.comboDefaultHardware.SelectedIndex;

                Properties.Settings.Default.enable_mqtt = settings_form.checkEnableMqtt.Checked;
                Properties.Settings.Default.mqtt_broker_host = settings_form.txtBrokerHost.Text;
                Properties.Settings.Default.mqtt_broker_port = Convert.ToInt32(settings_form.numBrokerPort.Value);
                Properties.Settings.Default.mqtt_topic_parent = settings_form.txtParentTopic.Text;

                // pluto control
                Properties.Settings.Default.enable_pluto = settings_form.checkPlutoControl.Checked;

                Properties.Settings.Default.Save();
                
                load_settings();
            }
        }

        private void qO100WidebandChatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            chatForm.Show();
            chatForm.Focus();
        }

        /*
        private void lblServiceName_TextChanged(object sender, EventArgs e)
        {
            if (setting_enable_spectrum)
            {
                // we have decoded a callsign
                string callsign = lblServiceName.Text;
                int offset = 0;

                //Int32.TryParse(current, out offset);
                if (videoSource.current_rf_input_1 == nim.NIM_INPUT_TOP)
                    offset = videoSource.current_offset_A;
                else
                    offset = videoSource.current_offset_B;

                if (callsign.Length > 0)
                {
                    double freq = videoSource.current_frequency_1 + offset;
                    freq = freq / 1000;
                    float sr = videoSource.current_sr_1;

                    debug("New Callsign: " + callsign + "," + freq.ToString() + "," + sr.ToString());
                    sigs.updateCurrentSignal(callsign, freq, sr);

                }
            }
        }
        */




        private void manageStoredFrequenciesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frequencyManagerForm freqForm = new frequencyManagerForm(stored_frequencies);
            freqForm.ShowDialog();

            save_stored_frequencies();
            rebuild_stored_frequencies();
        }

        /*
        private void lblChatSigReport_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            float freq = videoSource.current_frequency_1 + videoSource.current_offset_A;
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
        */

        private void manualToolStripMenuItem_Click(object sender, EventArgs e)
        {
            batc_spectrum.changeTuneMode(0);
            manualToolStripMenuItem.Checked = true;
            autoHoldToolStripMenuItem.Checked = false;
            autoTimedToolStripMenuItem.Checked = false;
        }

        private void autoTimedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            batc_spectrum.changeTuneMode(2);
            manualToolStripMenuItem.Checked = false;
            autoHoldToolStripMenuItem.Checked = false;
            autoTimedToolStripMenuItem.Checked = true;
        }

        private void autoHoldToolStripMenuItem_Click(object sender, EventArgs e)
        {
            batc_spectrum.changeTuneMode(3);
            manualToolStripMenuItem.Checked = false;
            autoHoldToolStripMenuItem.Checked = true;
            autoTimedToolStripMenuItem.Checked = false;
        }


        /*
        private void lblAdjust2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            uint carrier_offset = 0;

            if (UInt32.TryParse(lblFreqCar2.Text, out carrier_offset))
            {
                carrier_offset = carrier_offset / 1000;
                videoSource.change_frequency(2, (videoSource.current_frequency_2 + carrier_offset), videoSource.current_sr_2, videoSource.current_enable_lnb_supply, videoSource.current_enable_horiz_supply, videoSource.current_rf_input_2, videoSource.current_tone_22kHz_P1);
            }

        }
        */

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

        /*
        private void lblServiceName2_TextChanged(object sender, EventArgs e)
        {
            if (setting_enable_spectrum)
            {
                // we have decoded a callsign
                string callsign = lblServiceName2.Text;
                int offset = 0;

                //Int32.TryParse(current, out offset);

                if (videoSource.current_rf_input_2 == nim.NIM_INPUT_TOP)
                    offset = videoSource.current_offset_A;
                else
                    offset = videoSource.current_offset_B;

                if (callsign.Length > 0)
                {
                    double freq = videoSource.current_frequency_2 + offset;
                    freq = freq / 1000;
                    float sr = videoSource.current_sr_2;

                    debug("New Callsign: " + callsign + "," + freq.ToString() + "," + sr.ToString());
                    sigs.updateCurrentSignal(callsign, freq, sr);

                }
            }

        }
        */

        /*
        private void offToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            if (!videoSource.HardwareConnected)
                return;

            videoSource.set_polarization_supply(0, false, false);

            offToolStripMenuItem.Checked = true;
            vertical13VToolStripMenuItem.Checked = false;
            horizontal18VToolStripMenuItem.Checked = false;
            
        }

        private void vertical13VToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!videoSource.HardwareConnected)
                return;

            videoSource.set_polarization_supply(0, true, false);

            offToolStripMenuItem.Checked = false;
            vertical13VToolStripMenuItem.Checked = true;
            horizontal18VToolStripMenuItem.Checked = false;
        }

        private void horizontal18VToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!videoSource.HardwareConnected)
                return;

            videoSource.set_polarization_supply(0, true, true);

            offToolStripMenuItem.Checked = false;
            vertical13VToolStripMenuItem.Checked = false;
            horizontal18VToolStripMenuItem.Checked = true;
        }

        private void kHzToolStripMenuItem_Click(object sender, EventArgs e)
        {
            kHzToolStripMenuItem.Checked = !kHzToolStripMenuItem.Checked;
            videoSource.change_frequency(1, videoSource.current_frequency_1, videoSource.current_sr_1, videoSource.current_enable_lnb_supply, videoSource.current_enable_horiz_supply, videoSource.current_rf_input_1, kHzToolStripMenuItem.Checked);
        }

        */
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
            System.Diagnostics.Process.Start("https://www.zr6tg.co.za/adding-2nd-transport-to-batc-minitiouner-v2/");
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


        private void toolStripMenuItem7_Click(object sender, EventArgs e)
        {
            // detect ftdi devices
            //hardwareInfoForm hwForm = new hardwareInfoForm(videoSource.ftdi_hw);
            //hwForm.ShowDialog();
        }

        private void manageExternalToolsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            externalToolsManager etManager = new externalToolsManager(external_tools);
            etManager.ShowDialog();

            save_external_tools();
            rebuild_external_tools();
        }

        private void configureCallsignToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pluto_client.ConfigureCallsignAndReboot("ZR6TG");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ConfigureVideoLayout(2);
        }

        SplitterPanel[] videoPanels = new SplitterPanel[4];

        private OTMediaPlayer ConfigureVideoPlayer(int nr, int preference)
        {
            OTMediaPlayer player;
            switch (preference)
            {
                case 0: // vlc
                    var vlc_video_player = new LibVLCSharp.WinForms.VideoView();
                    vlc_video_player.Dock = DockStyle.Fill;
                    videoPanels[nr].Controls.Add(vlc_video_player);
                    player = new VLCMediaPlayer(vlc_video_player);
                    player.Initialize(videoSource.GetVideoDataQueue(nr));
                    return player;
                    
                case 1: // ffmpeg
                    var ffmpeg_video_player = new FlyleafLib.Controls.WinForms.FlyleafHost();
                    ffmpeg_video_player.Dock = DockStyle.Fill;
                    videoPanels[nr].Controls.Add(ffmpeg_video_player);
                    player = new FFMPEGMediaPlayer(ffmpeg_video_player);
                    player.Initialize(videoSource.GetVideoDataQueue(nr));
                    return player;
                case 2: // mpv
                    var mpv_video_player = new PictureBox();
                    mpv_video_player.Dock = DockStyle.Fill;
                    videoPanels[nr].Controls.Add(mpv_video_player);
                    player = new MPVMediaPlayer(mpv_video_player.Handle.ToInt64());
                    player.Initialize(videoSource.GetVideoDataQueue(nr));
                    return player;
            }

            return null;
        }

        private List<OTMediaPlayer> ConfigureMediaPlayers(int amount, int[] playerPreference)
        {
            List<OTMediaPlayer> mediaPlayers = new List<OTMediaPlayer>();

            videoPanels[0] = splitContainer4.Panel1;
            videoPanels[1] = splitContainer5.Panel1;
            videoPanels[2] = splitContainer4.Panel2;
            videoPanels[3] = splitContainer5.Panel2;

            for (int c = 0; c < amount; c++)
            {
                var media_player = ConfigureVideoPlayer(c, 2);
                mediaPlayers.Add(media_player);
            }

            ConfigureVideoLayout(amount);

            return mediaPlayers;
        }


        // configure video layout - only for 1, 2 or 4 video players 
        // todo: reset for changing sources
        private void ConfigureVideoLayout(int amount)
        {

            switch (amount)
            {
                case 1:
                    splitContainer4.Panel2Collapsed = true;
                    splitContainer4.Panel2.Hide();
                    splitContainer3.Panel2Collapsed = true;
                    splitContainer3.Panel2.Hide();
                    break;
                case 2:
                    splitContainer4.Panel2Collapsed = true;
                    splitContainer4.Panel2.Hide();
                    splitContainer5.Panel2Collapsed = true;
                    splitContainer5.Panel2.Hide();
                    break;

            }
        }

        private void btnSourceConnect_Click(object sender, EventArgs e)
        {
            if (!SourceConnect(_availableSources[comboAvailableSources.SelectedIndex]))
                return;

            if (setting_enable_spectrum)
            {
                debug("Settings: QO-100 Spectrum Enabled");

                this.DoubleBuffered = true;
                batc_spectrum = new BATCSpectrum(spectrum, videoSource.GetVideoSourceCount());
                batc_spectrum.OnSignalSelected += Batc_spectrum_OnSignalSelected;
            }
            else
            {
                debug("Settings: QO-100 Spectrum Disabled");

                splitContainer2.Panel2Collapsed = true;
                splitContainer2.Panel2.Enabled = false;
            }

        }

        private void btnSourceSettings_Click(object sender, EventArgs e)
        {
            _availableSources[comboAvailableSources.SelectedIndex].ShowSettings();
            sourceInfo.Text = _availableSources[comboAvailableSources.SelectedIndex].GetDescription();
        }

        private void comboAvailableSources_SelectedIndexChanged(object sender, EventArgs e)
        {
            sourceInfo.Text = _availableSources[comboAvailableSources.SelectedIndex].GetDescription();
        }
    }


}
