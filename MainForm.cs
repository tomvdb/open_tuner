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
using opentuner.ExtraFeatures.BATCWebchat;
using opentuner.ExtraFeatures.QuickTuneControl;
using opentuner.MediaSources.Winterhill;
using static opentuner.signal;
using System.Runtime.CompilerServices;
using Vortice.MediaFoundation;
using Serilog;

namespace opentuner
{
    delegate void updateNimStatusGuiDelegate(MainForm gui, TunerStatus new_status);
    delegate void updateTSStatusGuiDelegate(int device, MainForm gui, TSStatus new_status);
    delegate void updateMediaStatusGuiDelegate(int tuner, MainForm gui, MediaStatus new_status);
    delegate void UpdateLBDelegate(ListBox LB, Object obj);
    delegate void UpdateLabelDelegate(Label LB, Object obj);
    delegate void updateRecordingStatusDelegate(MainForm gui, bool recording_status, string id);

    public partial class MainForm : Form
    {
        // extras
        MqttManager mqtt_client;
        F5OEOPlutoControl pluto_client;
        BATCSpectrum batc_spectrum;
        BATCChat batc_chat;
        QuickTuneControl quickTune_control;

        private static List<OTMediaPlayer> _mediaPlayers;
        private static List<OTSource> _availableSources = new List<OTSource>();
        private static List<TSRecorder> _ts_recorders = new List<TSRecorder>();
        private static List<TSUdpStreamer> _ts_streamers = new List<TSUdpStreamer>();

        private static OTSource videoSource;

        // mqtt settings
        string setting_mqtt_broker_host = "127.0.0.1";
        int setting_mqtt_broker_port = 1883;
        string setting_mqtt_parent_topic = "";

        // f5oeoe firmware pluto
        bool setting_enable_pluto = false;

        private TunerControlForm tuner1ControlForm;
        private TunerControlForm tuner2ControlForm;

        private VideoViewForm mediaPlayer1Window;
        private VideoViewForm mediaPlayer2Window;

        private MainSettings _settings;
        private SettingsManager<MainSettings> _settingsManager;

        List<StoredFrequency> stored_frequencies = new List<StoredFrequency>();
        List<ExternalTool> external_tools = new List<ExternalTool>();


        public MainForm()
        {
            ThreadPool.GetMinThreads(out int workers, out int ports);
            ThreadPool.SetMinThreads(workers + 6, ports + 6);

            InitializeComponent();

            _settings = new MainSettings();
            _settingsManager = new SettingsManager<MainSettings>("open_tuner_settings");
            _settings = (_settingsManager.LoadSettings(_settings));

            //setup
            splitContainer2.Panel2Collapsed = true;
            splitContainer2.Panel2.Enabled = false;

            checkBatcSpectrum.Checked = _settings.enable_spectrum_checkbox;
            checkBatcChat.Checked = _settings.enable_chatform_checkbox;
            checkMqttClient.Checked = _settings.enable_mqtt_checkbox;
            checkQuicktune.Checked = _settings.enable_quicktune_checkbox;

            // load available sources
            _availableSources.Add(new MinitiounerSource());
            _availableSources.Add(new LongmyndSource());
            _availableSources.Add(new WinterhillSource());

            comboAvailableSources.Items.Clear();

            for (int c = 0; c < _availableSources.Count; c++)
            {
                comboAvailableSources.Items.Add(_availableSources[c].GetName());
            }

            comboAvailableSources.SelectedIndex = _settings.default_source;
            sourceInfo.Text = _availableSources[_settings.default_source].GetDescription();

        }

        /// <summary>
        /// Connect to Media Source and configure Media Players + extra's based on Media Source initialization.
        /// </summary>
        /// <param name="MediaSource"></param>
        private bool SourceConnect(OTSource MediaSource)
        {
            Log.Information("Connecting to " + MediaSource.GetName());

            videoSource = MediaSource;

            int video_players_required = videoSource.Initialize(ChangeVideo, PropertiesPage);

            if (video_players_required < 0)
            {
                Log.Error("Error Connecting MediaSource: " + videoSource.GetName());
                MessageBox.Show("Error Connecting MediaSource: " + videoSource.GetName());
                return false;
            }

            this.Text = this.Text += " - " + videoSource.GetDeviceName();

            // preferred player to use for each video view
            // 0 = vlc, 1 = ffmpeg, 2 = mpv
            Log.Information("Configuring Media Players");
            _mediaPlayers = ConfigureMediaPlayers(videoSource.GetVideoSourceCount(), _settings.mediaplayer_preferences );
            videoSource.ConfigureVideoPlayers(_mediaPlayers);
            videoSource.ConfigureMediaPath(_settings.media_path);


            // set recorders
            _ts_recorders = ConfigureTSRecorders(videoSource, _settings.media_path);
            videoSource.ConfigureTSRecorders(_ts_recorders);

            // set udp streamers
            _ts_streamers = ConfigureTSStreamers(videoSource, _settings.streamer_udp_hosts, _settings.streamer_udp_ports);
            videoSource.ConfigureTSStreamers(_ts_streamers);

            // update gui
            SourcePage.Hide();
            tabControl1.TabPages.Remove(SourcePage);

            videoSource.OnSourceData += VideoSource_OnSourceData;

            return true;
        }

        private void VideoSource_OnSourceData(Dictionary<string, string> Properties, string topic)
        {
            if (batc_spectrum != null)
            {
                if (Properties.ContainsKey("frequency") && Properties.ContainsKey("service_name") && Properties.ContainsKey("symbol_rate"))
                {
                    double freq = 0;
                    float sr = 0;
                    string callsign = Properties["service_name"];

                    if (double.TryParse(Properties["frequency"], NumberStyles.Any, CultureInfo.InvariantCulture, out freq))
                    {
                        if (float.TryParse(Properties["symbol_rate"], NumberStyles.Any, CultureInfo.InvariantCulture, out sr))
                        {
                            freq = freq / 1000;

                            batc_spectrum.updateSignalCallsign(callsign, freq, sr/1000);
                        }
                    }
                }
            }

            if (mqtt_client != null)
            {
                // send mqtt data
                mqtt_client.SendProperties(Properties, videoSource.GetName() + "/" + topic);
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
                {
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

        public void start_video(int video_number)
        {
            if (_mediaPlayers == null)
            {
                Log.Debug("Media player is still null");
                return;
            }

            if (video_number < _mediaPlayers.Count)
            {
                if (video_number == 0)
                {
                    Log.Information("Ping");
                }

                videoSource.StartStreaming(video_number);
                _mediaPlayers[video_number].Play();
            }
        }

        public void stop_video(int video_number)
        {
            if (_mediaPlayers == null)
            {
                return;
            }

            if (video_number < _mediaPlayers.Count)
            {
                videoSource.StopStreaming(video_number);
                _mediaPlayers[video_number].Stop();
            }
        }

        private void ChangeVideo(int video_number, bool start)
        {
            Log.Information("Change Video " + video_number.ToString());

            if (start)
                start_video(video_number-1);
            else 
                stop_video(video_number-1);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Log.Information("Exiting...");
            Log.Information("* Saving Settings");

            // save current windows properties
            _settings.gui_window_width = this.Width;
            _settings.gui_window_height = this.Height;
            _settings.gui_window_x = this.Left;
            _settings.gui_window_y = this.Top;
            _settings.gui_main_splitter_position = splitContainer1.SplitterDistance;

            _settingsManager.SaveSettings(_settings);

            try
            {
                /*
                if (mqtt_client !=  null)
                {
                    mqtt_client.Disconnect();
                }
                */

                if (batc_spectrum != null)
                    batc_spectrum.Close();

                if (batc_chat != null)
                    batc_chat.Close();

                if (quickTune_control != null)
                    quickTune_control.Close();

                Log.Information("* Stopping Playing Video");

                if (videoSource != null)
                {
                    ChangeVideo(1, false);
                    if (videoSource.GetVideoSourceCount() == 2)
                        ChangeVideo(2, false);
                }

                Log.Information("* Closing Extra TS Threads");

                // close ts streamers
                for (int c = 0; c < _ts_streamers.Count; c++)
                {
                    _ts_streamers[c].Close();
                }


                // close ts recorders
                for (int c = 0; c < _ts_recorders.Count; c++) 
                {
                    _ts_recorders[c].Close();
                }

                // close available media sources
                for (int c = 0; c < _availableSources.Count; c++)
                {
                    _availableSources[c].Close();
                }


                if (tuner1ControlForm != null)
                    tuner1ControlForm.Close();
                if (tuner2ControlForm != null)
                    tuner2ControlForm.Close();

            }
            catch ( Exception Ex)
            {
                // we are closing, we don't really care about exceptions at this point
                Log.Error( Ex, "Closing Exception");
            }

            Log.Information("Bye!");

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (_settings.media_path.Length == 0)
                _settings.media_path = AppDomain.CurrentDomain.BaseDirectory;

            if (_settings.gui_window_width != -1)
            {
                Log.Information("Restoring Window Positions:");
                Log.Information(" Size: (" + _settings.gui_window_height.ToString() + "," + _settings.gui_window_width.ToString() + ")");
                Log.Information(" Position: (" + _settings.gui_window_x.ToString() + "," + _settings.gui_window_y.ToString() + ")");

                this.Height = _settings.gui_window_height;
                this.Width = _settings.gui_window_width;

                this.Left = _settings.gui_window_x;
                this.Top = _settings.gui_window_y;
            }






            /*
            load_stored_frequencies();
            rebuild_stored_frequencies();

            load_external_tools();
            rebuild_external_tools();
            */

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

            /*
            // mqtt client
            setting_enable_mqtt = false;
            if (setting_enable_mqtt)
            {
                mqtt_client = new MqttManager(setting_mqtt_broker_host, setting_mqtt_broker_port, setting_mqtt_parent_topic);
                mqtt_client.OnMqttMessageReceived += Mqtt_client_OnMqttMessageReceived;

                // pluto - requires mqtt
                if (setting_enable_pluto)
                {
                    pluto_client = new F5OEOPlutoControl(mqtt_client);
                    plutoToolStripMenuItem.Visible = true;
                }
            }
            */
        }

        private void Batc_spectrum_OnSignalSelected(int Receiver, uint Freq, uint SymbolRate)
        {
            videoSource.SetFrequency(Receiver, Freq, SymbolRate, true);
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
                //et_menu.Click += Et_menu_Click;

                externalToolsToolStripMenuItem1.DropDownItems.Add(et_menu);
            }
        }

        /*
        private void Et_menu_Click(object sender, EventArgs e)
        {
            int tag = Convert.ToInt32(((ToolStripMenuItem)(sender)).Tag);
            Log.Information(tag.ToString());

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
        */

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
            Log.Information(tag.ToString());

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
                Log.Warning("Error reading tools.json - file missing, or wrong format :" + Ex.Message);
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
                Log.Information("Error reading frequencies.json - file missing, or wrong format :" + Ex.Message);
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
                Log.Information("Error writing tools file: " + Ex.Message);
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
                Log.Information("Error writing frequencies file: " + Ex.Message);
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
            settingsForm settings_form = new settingsForm(ref _settings);

            if (settings_form.ShowDialog() == DialogResult.OK)
            {
                _settingsManager.SaveSettings(_settings);
            }
        }

        private void qO100WidebandChatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (batc_chat != null)
                batc_chat.Show();
        }

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

        private void addingA2ndTransportToBATCMinitiounerV2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.zr6tg.co.za/adding-2nd-transport-to-batc-minitiouner-v2/");
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
                    Log.Information(nr.ToString() + " - " + "VLC");
                    var vlc_video_player = new LibVLCSharp.WinForms.VideoView();
                    vlc_video_player.Dock = DockStyle.Fill;
                    videoPanels[nr].Controls.Add(vlc_video_player);
                    player = new VLCMediaPlayer(vlc_video_player);
                    player.Initialize(videoSource.GetVideoDataQueue(nr), nr);
                    return player;
                    
                case 1: // ffmpeg
                    Log.Information(nr.ToString() + " - " + "FFMPEG");
                    var ffmpeg_video_player = new FlyleafLib.Controls.WinForms.FlyleafHost();
                    ffmpeg_video_player.Dock = DockStyle.Fill;
                    videoPanels[nr].Controls.Add(ffmpeg_video_player);
                    player = new FFMPEGMediaPlayer(ffmpeg_video_player);
                    player.Initialize(videoSource.GetVideoDataQueue(nr), nr);
                    return player;
                case 2: // mpv
                    Log.Information(nr.ToString() + " - " + "MPV");
                    var mpv_video_player = new PictureBox();
                    mpv_video_player.Dock = DockStyle.Fill;
                    videoPanels[nr].Controls.Add(mpv_video_player);
                    player = new MPVMediaPlayer(mpv_video_player.Handle.ToInt64());
                    player.Initialize(videoSource.GetVideoDataQueue(nr), nr);
                    return player;
            }

            return null;
        }

        // configure TS recorders
        private List<TSRecorder> ConfigureTSRecorders(OTSource video_source, string video_path)
        {
            List<TSRecorder> tSRecorders = new List<TSRecorder>();

            for (int c = 0; c < video_source.GetVideoSourceCount(); c++)
            {
                var ts_recorder = new TSRecorder(video_path, c, video_source);
                tSRecorders.Add(ts_recorder);
            }

            return tSRecorders;
        }

        // configure TS streamers
        private List<TSUdpStreamer> ConfigureTSStreamers(OTSource video_source, string[] udpHosts, int[] udpPorts )
        {
            List<TSUdpStreamer> tsStreamers = new List<TSUdpStreamer>();

            for (int c= 0; c < videoSource.GetVideoSourceCount(); c++)
            {
                var ts_streamer = new TSUdpStreamer(udpHosts[c], udpPorts[c], c, video_source);
                tsStreamers.Add(ts_streamer);   
            }

            return tsStreamers;
        }


        // configure media players
        private List<OTMediaPlayer> ConfigureMediaPlayers(int amount, int[] playerPreference)
        {
            List<OTMediaPlayer> mediaPlayers = new List<OTMediaPlayer>();

            if (amount == 4)
            {
                videoPanels[0] = splitContainer4.Panel1;
                videoPanels[2] = splitContainer5.Panel1;
                videoPanels[1] = splitContainer4.Panel2;
                videoPanels[3] = splitContainer5.Panel2;
            }
            else
            {
                videoPanels[0] = splitContainer4.Panel1;
                videoPanels[1] = splitContainer5.Panel1;
                videoPanels[2] = splitContainer4.Panel2;
                videoPanels[3] = splitContainer5.Panel2;
            }

            for (int c = 0; c < amount; c++)
            {
                var media_player = ConfigureVideoPlayer(c, playerPreference[c]);
                mediaPlayers.Add(media_player);
            }

            ConfigureVideoLayout(amount);

            return mediaPlayers;
        }


        // configure video layout - only for 1, 2 or 4 video players 
        // todo: reset for changing sources
        private void ConfigureVideoLayout(int amount)
        {
            splitContainer3.Visible = true;
            splitContainer4.Visible = true;
            splitContainer5.Visible = true;

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

            if (checkBatcSpectrum.Checked)
            {
                // show spectrum
                splitContainer2.Panel2Collapsed = false;
                splitContainer2.Panel2.Enabled = true;

                this.DoubleBuffered = true;
                batc_spectrum = new BATCSpectrum(spectrum, videoSource.GetVideoSourceCount());
                batc_spectrum.OnSignalSelected += Batc_spectrum_OnSignalSelected;
            }

            if (checkBatcChat.Checked)
            {
                qO100WidebandChatToolStripMenuItem.Visible = true;
                batc_chat = new BATCChat(videoSource);
            }

            if (checkQuicktune.Checked)
            {
                quickTune_control = new QuickTuneControl(videoSource);
            }

            if (checkMqttClient.Checked)
            {
                mqtt_client = new MqttManager();
            }

            splitContainer1.SplitterDistance = _settings.gui_main_splitter_position;

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

        private void checkMqttClient_CheckedChanged(object sender, EventArgs e)
        {
            _settings.enable_mqtt_checkbox = checkMqttClient.Checked;
        }

        private void checkQuicktune_CheckedChanged(object sender, EventArgs e)
        {
            _settings.enable_quicktune_checkbox = checkQuicktune.Checked;
        }

        private void checkBatcSpectrum_CheckedChanged(object sender, EventArgs e)
        {
            _settings.enable_spectrum_checkbox = checkBatcSpectrum.Checked;
        }

        private void checkBatcChat_CheckedChanged(object sender, EventArgs e)
        {
            _settings.enable_chatform_checkbox = checkBatcChat.Checked;
        }

        private void linkBatcWebchatSettings_Click(object sender, EventArgs e)
        {
            // webchat settings
            WebChatSettings wc_settings = new WebChatSettings();
            SettingsManager<WebChatSettings> wc_settingsManager = new SettingsManager<WebChatSettings>("qo100_webchat_settings");
            wc_settings = (wc_settingsManager.LoadSettings(wc_settings));

            WebChatSettngsForm wc_settings_form = new WebChatSettngsForm(ref wc_settings);

            if (wc_settings_form.ShowDialog() == DialogResult.OK)
            {
                wc_settingsManager.SaveSettings(wc_settings);
            }

        }

        private void linkDocumentation_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.zr6tg.co.za/opentuner-documentation/");
        }

        private void linkMqttSettings_Click(object sender, EventArgs e)
        {
            // mqtt settings
            MqttManagerSettings mqtt_settings = new MqttManagerSettings();
            SettingsManager<MqttManagerSettings> mqtt_settingsManager = new SettingsManager<MqttManagerSettings>("mqttclient_settings");
            mqtt_settings = mqtt_settingsManager.LoadSettings(mqtt_settings);

            MqttSettingsForm mqtt_settings_form = new MqttSettingsForm(ref mqtt_settings);

            if (mqtt_settings_form.ShowDialog() == DialogResult.OK)
            {
                mqtt_settingsManager.SaveSettings(mqtt_settings);
            }
        }

        private void linkQuickTuneSettings_Click(object sender, EventArgs e)
        {
            // quick tune settings
            QuickTuneControlSettings quicktune_settings = new QuickTuneControlSettings();
            SettingsManager<QuickTuneControlSettings> quicktune_settingsManager = new SettingsManager<QuickTuneControlSettings>("quicktune_settings");
            quicktune_settings = quicktune_settingsManager.LoadSettings(quicktune_settings);

            QuickTuneControlSettingsForm quicktune_settings_form = new QuickTuneControlSettingsForm(ref quicktune_settings);

            if (quicktune_settings_form.ShowDialog() == DialogResult.OK)
            {
                quicktune_settingsManager.SaveSettings(quicktune_settings);
            }
        }

        private void linkSpectrumDocumentation_Click(object sender, EventArgs e)
        {           
            System.Diagnostics.Process.Start("https://www.zr6tg.co.za/opentuner-spectrum/");
        }

        private void LinkMqttDocumentation_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.zr6tg.co.za/opentuner-mqtt-client/");
        }

        private void linkQuickTuneDocumentation_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.zr6tg.co.za/opentuner-quicktune-control/");
        }

        private void linkBatcWebchatDocumentation_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.zr6tg.co.za/opentuner-webchat/");
        }

        private void linkOpenTunerUpdates_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.zr6tg.co.za/open-tuner/");
        }

        private void linkSourceMoreInfo_Click(object sender, EventArgs e)
        {
            if (_availableSources[comboAvailableSources.SelectedIndex].GetMoreInfoLink().Length > 0 ) 
            {
                System.Diagnostics.Process.Start(_availableSources[comboAvailableSources.SelectedIndex].GetMoreInfoLink());
            }
        }

        private void linkGithubIssues_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/tomvdb/open_tuner/issues");

        }

        private void linkForum_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://forum.batc.org.uk/viewforum.php?f=142");

        }

        private void linkSupport_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.buymeacoffee.com/zr6tg/");
        }

        private void linkBatc_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://batc.org.uk/");

        }

        private void link2ndTS_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.zr6tg.co.za/adding-2nd-transport-to-batc-minitiouner-v2/");

        }

        private void linkPicoTuner_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.zr6tg.co.za/2024/02/11/picotuner-an-experimental-dual-ts-alternative/");
        }
    }


}
