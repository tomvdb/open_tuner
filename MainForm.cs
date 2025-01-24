using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using System.Globalization;
using System.Diagnostics;

using opentuner.MediaSources;
using opentuner.MediaSources.Minitiouner;
using opentuner.MediaSources.Longmynd;
using opentuner.MediaSources.Winterhill;

using opentuner.MediaPlayers;
using opentuner.MediaPlayers.MPV;
using opentuner.MediaPlayers.FFMPEG;
using opentuner.MediaPlayers.VLC;

using opentuner.Utilities;
using opentuner.Transmit;
using opentuner.ExtraFeatures.BATCSpectrum;
using opentuner.ExtraFeatures.BATCWebchat;
using opentuner.ExtraFeatures.MqttClient;
using opentuner.ExtraFeatures.QuickTuneControl;
using opentuner.ExtraFeatures.DATVReporter;

using Serilog;
using System.Runtime.CompilerServices;
using System.Drawing;
using opentuner.SettingsManagement;

namespace opentuner
{
    delegate void updateNimStatusGuiDelegate(MainForm gui, TunerStatus new_status);
    delegate void updateTSStatusGuiDelegate(int device, MainForm gui, TSStatus new_status);
    delegate void updateMediaStatusGuiDelegate(int tuner, MainForm gui, MediaStatus new_status);
    delegate void UpdateLBDelegate(ListBox LB, Object obj);
    delegate void UpdateLabelDelegate(Label LB, Object obj);
    delegate void updateRecordingStatusDelegate(MainForm gui, bool recording_status, string id);

    delegate void UpdateInfoDelegate(StreamInfoContainer info_object, OTSourceData info);

    public partial class MainForm : Form, IMessageFilter
    {
        // extras
        MqttManager mqtt_client;
        F5OEOPlutoControl pluto_client;
        BATCSpectrum batc_spectrum;
        BATCChat batc_chat;
        QuickTuneControl quickTune_control;
        DATVReporter datv_reporter = new DATVReporter();

        private static List<OTMediaPlayer> _mediaPlayers;
        private static List<OTSource> _availableSources = new List<OTSource>();
        private static List<TSRecorder> _ts_recorders = new List<TSRecorder>();
        private static List<TSUdpStreamer> _ts_streamers = new List<TSUdpStreamer>();

        private static OTSource videoSource;

        private MainSettings _settings;
        private SettingsManager<MainSettings> _settingsManager;

        private bool _settings_save = true;

        SettingsManager<List<StoredFrequency>> frequenciesManager;

        List<StoredFrequency> stored_frequencies = new List<StoredFrequency>();
        List<ExternalTool> external_tools = new List<ExternalTool>();

        List<VolumeInfoContainer> volume_display = new List<VolumeInfoContainer>();
        List<StreamInfoContainer> info_display = new List<StreamInfoContainer>();

        private bool source_connected = false;

        SplitterPanel[] video_panels = new SplitterPanel[4];

        bool properties_hidden = false;
        bool ExtraTool_hidden = false;

        public void UpdateInfo(StreamInfoContainer info_object, OTSourceData info)
        {

            if (info_object == null)
                return;

            if (info == null)
                return;

            if (info_object.InvokeRequired)
            {
                UpdateInfoDelegate ulb = new UpdateInfoDelegate(UpdateInfo);

                info_object?.Invoke(ulb, new object[] { info_object, info });
            }
            else
            {
                info_object.UpdateInfo(info);
            }

        }

        void ParseCommandLineOptions(string[] args)
        {
            int i = 0;

            while ( i < args.Length )
            {
                switch (args[i])
                {
                    case "--nosave":
                        _settings_save = false;
                        break;
                    case "--autoconnect":
                        _settings.auto_connect = true;
                        break;

                    case "--noconnect":
                        _settings.auto_connect = false;
                        break;

                    case "--enablebatcspectrum":
                        _settings.enable_spectrum_checkbox = true; 
                        break;

                    case "--disablebatcspectrum":
                        _settings.enable_spectrum_checkbox = false;
                        break;

                    case "--enablebatcchat":
                        _settings.enable_chatform_checkbox = true;
                        break;

                    case "--disablebatcchat":
                        _settings.enable_chatform_checkbox = false;
                        break;

                    case "--enablemqtt":
                        _settings.enable_mqtt_checkbox = true;
                        break;

                    case "--disablemqtt":
                        _settings.enable_mqtt_checkbox = false;
                        break;

                    case "--enablequicktune":
                        _settings.enable_quicktune_checkbox = true;
                        break;

                    case "--disablequicktune":
                        _settings.enable_quicktune_checkbox = false;
                        break;

                    case "--enabledatvreporter":
                        _settings.enable_datvreporter_checkbox = true;
                        break;

                    case "--disabledatvreporter":
                        _settings.enable_datvreporter_checkbox = false;
                        break;

                    case "--hideproperties":
                        _settings.hide_properties = true;
                        break;

                    case "--showproperties":
                        _settings.hide_properties = false;
                        break;

                    case "--showextratools":
                        _settings.hide_ExtraTool = false;
                        break;

                    case "--hideextratools":
                        _settings.hide_ExtraTool = true;
                        break;

                    case "--hidevideoinfo":
                        for (int j = 0; j < 4; j++)
                            _settings.show_video_overlays[j] = false;
                        break;

                    case "--showvideoinfo":
                        for (int j = 0; j < 4; j++)
                            _settings.show_video_overlays[j] = true;
                        break;

                    case "--windowwidth":
                        
                        int new_window_width = 0;

                        if (int.TryParse(args[i+1], out new_window_width))
                        {
                            _settings.gui_window_width = new_window_width;
                        }
                        else
                        {
                            Log.Error(args[i + 1] + " is not a valid window width. Integer Expected");
                        }

                        i += 1;

                        break;

                    case "--windowheight":

                        int new_window_height = 0;

                        if (int.TryParse(args[i + 1], out new_window_height))
                        {
                            _settings.gui_window_height = new_window_height;
                        }
                        else
                        {
                            Log.Error(args[i + 1] + " is not a valid window height. Integer Expected");
                        }

                        i += 1;

                        break;

                    case "--windowx":
                        int new_window_x = 0;

                        if (int.TryParse(args[i + 1], out new_window_x))
                        {
                            _settings.gui_window_x = new_window_x;
                        }
                        else
                        {
                            Log.Error(args[i + 1] + " is not a valid window x. Integer Expected");
                        }

                        i += 1;

                        break;

                    case "--windowy":
                        int new_window_y = 0;

                        if (int.TryParse(args[i + 1], out new_window_y))
                        {
                            _settings.gui_window_y = new_window_y;
                        }
                        else
                        {
                            Log.Error(args[i + 1] + " is not a valid window y. Integer Expected");
                        }

                        i += 1;

                        break;


                    case "--defaultsource":

                        int new_default_source = 0;

                        if (int.TryParse(args[i + 1], out new_default_source))
                        {
                            if (new_default_source < 3 && new_default_source >= 0)
                            {
                                _settings.default_source = new_default_source;
                            }
                            else
                            {
                                Log.Error(args[i + 1] + " is not a valid default source parameter. 0-2 Expected");
                            }
                        }
                        else
                        {
                            Log.Error(args[i + 1] + " is not a valid default source parameter. 0-2 Expected");
                        }
                        
                        i += 1;

                        break;

                    default:
                        Log.Warning("Unknown command line param: " + args[i]);
                        break;
                }

                // grab next param
                i += 1;
            }
        }

        public MainForm(string[] args)
        {
            ThreadPool.GetMinThreads(out int workers, out int ports);
            ThreadPool.SetMinThreads(workers + 6, ports + 6);

            InitializeComponent();

            Application.AddMessageFilter(this);

            _settings = new MainSettings();

            //SettingsFormBuilder test = new SettingsFormBuilder(_settings);

            _settingsManager = new SettingsManager<MainSettings>("open_tuner_settings");
            _settings = (_settingsManager.LoadSettings(_settings));

            // parse command line options
            ParseCommandLineOptions(args);

            //setup
            splitContainer2.Panel2Collapsed = true;
            splitContainer2.Panel2.Enabled = false;

            checkBatcSpectrum.Checked = _settings.enable_spectrum_checkbox;
            checkBatcChat.Checked = _settings.enable_chatform_checkbox;
            checkMqttClient.Checked = _settings.enable_mqtt_checkbox;
            checkQuicktune.Checked = _settings.enable_quicktune_checkbox;
            checkDATVReporter.Checked = _settings.enable_datvreporter_checkbox;

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

            // load stored presets
            frequenciesManager = new SettingsManager<List<StoredFrequency>>("frequency_presets");
            stored_frequencies = frequenciesManager.LoadSettings(stored_frequencies);

            var compileTime = new DateTime(Builtin.CompileTime, DateTimeKind.Utc);
            DateTimeFormatInfo usDateFormat = new CultureInfo("en-US", false).DateTimeFormat;
            string compileTime_usFormat = compileTime.ToString("u", usDateFormat);

            Text = "Open Tuner (ZR6TG) - Version: " + GlobalDefines.Version + " - Build: " + compileTime_usFormat;
            Log.Information(Text);
        }

        /// <summary>
        /// Connect to Media Source and configure Media Players + extra's based on Media Source initialization.
        /// </summary>
        /// <param name="MediaSource"></param>
        private bool SourceConnect(OTSource MediaSource, bool mute_at_startup)
        {
            Log.Information("Connecting to " + MediaSource.GetName());

            videoSource = MediaSource;

            int video_players_required = videoSource.Initialize(ChangeVideo, PropertiesPage, mute_at_startup);

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
            _mediaPlayers = ConfigureMediaPlayers(videoSource.GetVideoSourceCount(), _settings.mediaplayer_preferences, _settings.mediaplayer_windowed );
            videoSource.ConfigureVideoPlayers(_mediaPlayers);
            videoSource.ConfigureMediaPath(_settings.media_path);


            // set recorders
            _ts_recorders = ConfigureTSRecorders(videoSource, _settings.media_video_path);
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

        private void VideoSource_OnSourceData(int video_nr, OTSourceData properties, string description)
        {
            
            if (video_nr < info_display.Count && video_nr >= 0)
            {
                if (info_display[video_nr] != null)
                    UpdateInfo(info_display[video_nr], properties);
            }
            else
            {
                Log.Error("info_display count does not fit video_nr");
            }

            if (datv_reporter != null && properties.demod_locked)
            {
                bool result = datv_reporter.SendISawMessage(new ISawMessage(
                    properties.service_name,
                    properties.db_margin,
                    properties.mer,
                    properties.frequency,
                    properties.symbol_rate,
                    videoSource.GetDeviceName()
                    ));

                /*
                if (!result)
                {
                    if (!datv_reporter.Connected)
                    {
                        datv_reporter.Connect();
                    }
                }
                */
            }

            if (batc_spectrum != null)
            {
                int tuner = properties.video_number;
                float freq = properties.frequency;
                float sr = properties.symbol_rate;
                string callsign = properties.service_name;

                batc_spectrum.updateTuner(tuner, Math.Round(freq / 1000.0f,3), sr / 1000, properties.demod_locked);
                if (properties.demod_locked)
                {
                    //Log.Information(callsign.ToString() + "," + freq.ToString() + "," + sr.ToString());

                    batc_spectrum.updateSignalCallsign(tuner, callsign, Math.Round(freq / 1000.0f,3), sr / 1000);
                }
            }

            if (mqtt_client != null)
            {
                // send mqtt data
                mqtt_client.SendProperties(properties, videoSource.GetName() + "/" + description);
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
                batc_spectrum?.updateStreaming(video_number, true);
                _mediaPlayers[video_number].Play();
                // Start with volume "muted".
                // The real audio volume is set later.
                // Reason for muting here:
                //   If audio is muted for the tuner and stream
                //   we start with Volume 100 or so here and get an annoying audio glitch.
                //   (detected with MPV player)
                _mediaPlayers[video_number].SetVolume(0);
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
                batc_spectrum?.updateStreaming(video_number, false);
                _mediaPlayers[video_number].Stop();
            }
        }

        private void ChangeVideo(int video_number, bool start)
        {
            Log.Information("Change Video " + video_number.ToString());

            if (start)
                start_video(video_number);
            else 
                stop_video(video_number);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Log.Information("Exiting...");

            Application.RemoveMessageFilter(this);

            Log.Information("* Saving Settings");

            // save current windows properties
            _settings.gui_window_width = this.Width;
            _settings.gui_window_height = this.Height;
            _settings.gui_window_x = this.Left;
            _settings.gui_window_y = this.Top;
            _settings.gui_main_splitter_position = splitContainer1.SplitterDistance;

            if (_settings_save)
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

                if (datv_reporter != null)
                    datv_reporter.Close();

                Log.Information("* Stopping Playing Video");

                if (videoSource != null)
                {
                    for (int i = 0; i < videoSource.GetVideoSourceCount(); i++)
                    {
                        ChangeVideo(i, false);
                        _mediaPlayers?[i]?.Close();
                    }
                }

                Log.Information("* Closing Extra TS Threads");

                // close ts streamers
                for (int c = 0; c < _ts_streamers.Count; c++)
                {
                    _ts_streamers?[c]?.Close();
                }


                // close ts recorders
                for (int c = 0; c < _ts_recorders.Count; c++) 
                {
                    _ts_recorders?[c]?.Close();
                }

                // close available media sources
                for (int c = 0; c < _availableSources.Count; c++)
                {
                    _availableSources?[c]?.Close();
                }

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

                //align to current available screens
                
                //first: create a virtual screen area as a combination of all screens
                Rectangle vScreenRect = new Rectangle(0, 0, 0, 0);
                int vScreenRight = 0;
                int vScreenBottom = 0;
                Screen[] screens = System.Windows.Forms.Screen.AllScreens;
                foreach(Screen s in screens)
                {
                    Log.Information(s.ToString());
                    if (s.WorkingArea.Top < vScreenRect.Top)
                        vScreenRect.Y = s.WorkingArea.Top;
                    if (s.WorkingArea.Left < vScreenRect.Left)
                        vScreenRect.X = s.WorkingArea.Left;
                    if (s.WorkingArea.Bottom > vScreenBottom)
                        vScreenBottom = s.WorkingArea.Bottom;
                    if (s.WorkingArea.Right > vScreenRight)
                        vScreenRight = s.WorkingArea.Right;

                }
                vScreenRect.Height = vScreenBottom - vScreenRect.Top;
                vScreenRect.Width = vScreenRight - vScreenRect.Left;

                //second: if necessary align the gui window to fit into the virual screen area
                if (this.Top < vScreenRect.Top)
                    this.Top = vScreenRect.Top;
                if (this.Left < vScreenRect.Left)
                    this.Left = vScreenRect.Left;
                if (this.Height > vScreenRect.Height)
                    this.Height = vScreenRect.Height;
                if (this.Width > vScreenRect.Width)
                    this.Width = vScreenRect.Width;

                //third: ensure that the menu bar is visible on one screen
                //       to make resize etc. possible
                // not yet implemented (necessary?)

                // it is intended not to save the aligned position here

                Log.Information("Aligned Window Positions:");
                Log.Information(" Size: (" + _settings.gui_window_height.ToString() + "," + _settings.gui_window_width.ToString() + ")");
                Log.Information(" Position: (" + _settings.gui_window_x.ToString() + "," + _settings.gui_window_y.ToString() + ")");
            }


            // auto connect if specified
            if (_settings.auto_connect)
            {
                source_connected = ConnectSelectedSource();
            }



            // hide/show video overlay

        }

        private void Batc_spectrum_OnSignalSelected(int Receiver, uint Freq, uint SymbolRate)
        {
            stop_video(Receiver);
            batc_spectrum.switchTuner(Receiver, Convert.ToDouble(Freq) / 1000.0f, SymbolRate / 1000.0f);
            videoSource.SetFrequency(Receiver, Freq, SymbolRate, true);
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

        private void dATVReporterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // URL to open
            string url = "https://www.datvreporter.info/";

            // Open the default web browser with the specified URL
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }

        private void configureCallsignToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pluto_client.ConfigureCallsignAndReboot("ZR6TG");
        }

        private void LoadSeperateWindow(Control VideoControl, string Title, int Nr, Control[] extraControls)
        {
            var external_video_form = new VideoViewForm(VideoControl, Title, Nr, videoSource, extraControls);
            external_video_form.Show();
        }

        private OTMediaPlayer ConfigureVideoPlayer(int nr, int preference, bool seperate_window)
        {
            OTMediaPlayer player = null;

            VolumeInfoContainer video_volume_display = null;
            StreamInfoContainer video_info_display = null;

            video_volume_display = new VolumeInfoContainer();
            video_volume_display.Tag = nr;

            video_info_display = new StreamInfoContainer(_settings.show_video_overlays[nr]);
            video_info_display.Tag = nr;


            switch (preference)
            {
                case 0: // vlc
                    Log.Information(nr.ToString() + " - " + "VLC");
                    var vlc_video_player = new LibVLCSharp.WinForms.VideoView();
                    vlc_video_player.Dock = DockStyle.Fill;
                    vlc_video_player.MouseClick += video_player_MouseClick;
                    vlc_video_player.MouseDoubleClick += video_player_MouseDoubleClick;
                    vlc_video_player.MouseWheel += video_player_MouseWheel;
                    vlc_video_player.Tag = nr;


                    if (seperate_window)
                    {
                        LoadSeperateWindow(vlc_video_player, "VLC - " + (nr + 1).ToString(), nr, new Control[] {video_volume_display, video_info_display});
                    }
                    else
                    {
                        video_panels[nr].Controls.Add(video_volume_display);
                        video_panels[nr].Controls.Add(video_info_display);
                        video_panels[nr].Controls.Add(vlc_video_player);
                    }

                    player = new VLCMediaPlayer(vlc_video_player);
                    player.Initialize(videoSource.GetVideoDataQueue(nr), nr);
                    break;
                    
                case 1: // ffmpeg
                    Log.Information(nr.ToString() + " - " + "FFMPEG");
                    var ffmpeg_video_player = new FlyleafLib.Controls.WinForms.FlyleafHost();
                    ffmpeg_video_player.ToggleFullScreenOnDoubleClick = false;
                    ffmpeg_video_player.Dock = DockStyle.Fill;
                    ffmpeg_video_player.MouseClick += video_player_MouseClick;
                    ffmpeg_video_player.MouseDoubleClick += video_player_MouseDoubleClick;
                    ffmpeg_video_player.MouseWheel += video_player_MouseWheel;
                    ffmpeg_video_player.Tag = nr;


                    if (seperate_window)
                    {
                        LoadSeperateWindow(ffmpeg_video_player, "FFMPEG - " + (nr + 1).ToString(), nr, new Control[] { video_volume_display, video_info_display });
                    }
                    else
                    {
                        video_panels[nr].Controls.Add(video_volume_display);
                        video_panels[nr].Controls.Add(video_info_display);
                        video_panels[nr].Controls.Add(ffmpeg_video_player);
                    }

                    player = new FFMPEGMediaPlayer(ffmpeg_video_player);
                    player.Initialize(videoSource.GetVideoDataQueue(nr), nr);
                    break;

                case 2: // mpv
                    Log.Information(nr.ToString() + " - " + "MPV");
                    var mpv_video_player = new PictureBox();
                    mpv_video_player.Dock = DockStyle.Fill;
                    mpv_video_player.MouseClick += video_player_MouseClick;
                    mpv_video_player.MouseDoubleClick += video_player_MouseDoubleClick;
                    mpv_video_player.MouseWheel += video_player_MouseWheel;
                    mpv_video_player.Tag = nr;



                    if (seperate_window)
                    {
                        LoadSeperateWindow(mpv_video_player, "MPV - " + (nr + 1).ToString(), nr, new Control[] { video_volume_display, video_info_display});
                    }
                    else
                    {
                        video_panels[nr].Controls.Add(video_volume_display);
                        video_panels[nr].Controls.Add(video_info_display);
                        video_panels[nr].Controls.Add(mpv_video_player);
                    }

                    player = new MPVMediaPlayer(mpv_video_player.Handle.ToInt64());
                    player.Initialize(videoSource.GetVideoDataQueue(nr), nr);
                    break;
            }


            if (video_volume_display != null)
                volume_display.Add(video_volume_display);

            if (video_info_display != null)
                info_display.Add(video_info_display);

            return player;
        }

        private void video_player_MouseWheel(object sender, MouseEventArgs e)
        {
            int wheel_volume_rate = 10;  // todo, maybe turn this into a setting

            int video_nr = (int)((Control)sender).Tag;

            //if muted: unmute and restore last volume value with first wheel action.
            if (videoSource.GetMuteState(video_nr))
            {
                videoSource.ToggleMute(video_nr);
            }
            else
            {
                if (e.Delta < 0)
                {
                    videoSource.UpdateVolume(video_nr, -1 * wheel_volume_rate);
                }
                if (e.Delta > 0)
                {
                    videoSource.UpdateVolume(video_nr, wheel_volume_rate);
                }
            }

            if (volume_display.Count > video_nr)
            {
                volume_display[video_nr].UpdateVolume(videoSource.GetVolume(video_nr));
            }
        }

        private void video_player_MouseClick(object sender, MouseEventArgs e)
        {
            int video_nr = (int)((Control)sender).Tag;

            if (e.Button == MouseButtons.Left)
            {
                if (info_display.Count > video_nr)
                {
                    if (System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.R))
                    {
                        //Log.Information("video_player_MouseClick: 'R' pressed");
                        videoSource.InvokeOnMediaButtonPressed("media_controls_" + video_nr.ToString(), 2);
                    }
                    else if (System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.S))
                    {
                        //Log.Information("video_player_MouseClick: 'S' pressed");
                        videoSource.InvokeOnMediaButtonPressed("media_controls_" + video_nr.ToString(), 1);
                    }
                    else if (System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.U))
                    {
                        //Log.Information("video_player_MouseClick: 'U' pressed");
                        videoSource.InvokeOnMediaButtonPressed("media_controls_" + video_nr.ToString(), 3);
                    }
                    else if (info_display[video_nr] != null)
                        _settings.show_video_overlays[video_nr] = info_display[video_nr].Visible = !info_display[video_nr].Visible;
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (info_display.Count > video_nr)
                {
                    videoSource.ToggleMute(video_nr);
                    if (volume_display.Count > video_nr)
                    {
                        volume_display[video_nr].UpdateVolume(videoSource.GetVolume(video_nr));
                    }
                }
            }
        }

        bool fullScreen = false;

        private void resetFullView()
        {
            switch (videoSource.GetVideoSourceCount())
            {
                case 2:
                    splitContainer3.Panel1Collapsed = false;
                    splitContainer3.Panel2Collapsed = false;
                    splitContainer3.Panel1.Show();
                    splitContainer3.Panel2.Show();
                    break;
                case 4:
                    splitContainer3.Panel1Collapsed = false;
                    splitContainer3.Panel2Collapsed = false;
                    splitContainer4.Panel1Collapsed = false;
                    splitContainer4.Panel2Collapsed = false;
                    splitContainer5.Panel1Collapsed = false;
                    splitContainer5.Panel2Collapsed = false;
                    splitContainer3.Panel1.Show();
                    splitContainer3.Panel2.Show();
                    splitContainer4.Panel1.Show();
                    splitContainer4.Panel2.Show();
                    splitContainer5.Panel1.Show();
                    splitContainer5.Panel2.Show();
                    break;
                default:
                    break;
            }
        }

        private void changeView(int video_nr)
        {
            if (fullScreen == true)
            {
                resetFullView();
                fullScreen = false;
            }
            else
            {
                fullScreen = true;
                switch (videoSource.GetVideoSourceCount())
                {
                    case 2:
                        switch (video_nr)
                        {
                            case 0:
                                splitContainer3.Panel2Collapsed = true;
                                splitContainer3.Panel2.Hide();
                                break;
                            case 1:
                                splitContainer3.Panel1Collapsed = true;
                                splitContainer3.Panel1.Hide();
                                break;
                        }
                        break;
                    case 4:
                        switch (video_nr)
                        {
                            case 0:
                                splitContainer3.Panel2Collapsed = true;
                                splitContainer3.Panel2.Hide();
                                splitContainer4.Panel2Collapsed = true;
                                splitContainer4.Panel2.Hide();
                                break;
                            case 1:
                                splitContainer3.Panel2Collapsed = true;
                                splitContainer3.Panel2.Hide();
                                splitContainer4.Panel1Collapsed = true;
                                splitContainer4.Panel1.Hide();
                                break;
                            case 2:
                                splitContainer3.Panel1Collapsed = true;
                                splitContainer3.Panel1.Hide();
                                splitContainer5.Panel2Collapsed = true;
                                splitContainer5.Panel2.Hide();
                                break;
                            case 3:
                                splitContainer3.Panel1Collapsed = true;
                                splitContainer3.Panel1.Hide();
                                splitContainer5.Panel1Collapsed = true;
                                splitContainer5.Panel1.Hide();
                                break;
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private void video_player_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int video_nr = (int)((Control)sender).Tag;

            // restore info_display changed by first mouse click
            if (info_display[video_nr] != null)
                _settings.show_video_overlays[video_nr] = info_display[video_nr].Visible = !info_display[video_nr].Visible;

            if (videoSource.GetVideoSourceCount() > 1 && _settings.mediaplayer_windowed[video_nr] == false)
                changeView(video_nr);
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
        private List<OTMediaPlayer> ConfigureMediaPlayers(int amount, int[] playerPreference, bool[] windowed)
        {
            List<OTMediaPlayer> mediaPlayers = new List<OTMediaPlayer>();

            if (amount == 4)
            {
                video_panels[0] = splitContainer4.Panel1;
                video_panels[2] = splitContainer5.Panel1;
                video_panels[1] = splitContainer4.Panel2;
                video_panels[3] = splitContainer5.Panel2;
            }
            else
            {
                video_panels[0] = splitContainer4.Panel1;
                video_panels[1] = splitContainer5.Panel1;
                video_panels[2] = splitContainer4.Panel2;
                video_panels[3] = splitContainer5.Panel2;
            }

            for (int c = 0; c < amount; c++)
            {
                var media_player = ConfigureVideoPlayer(c, playerPreference[c], windowed[c]);
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

        private void DisconnectCurrentSource()
        {
            toolstripConnectToggle.Text = "Connect Source";
        }

        private bool ConnectSelectedSource()
        {
            if (!SourceConnect(_availableSources[comboAvailableSources.SelectedIndex], _settings.mute_at_startup))
                return false;

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

            if (checkDATVReporter.Checked)
            {
                dATVReporterToolStripMenuItem.Visible = true;
                if (!datv_reporter.Connect())
                {
                    Log.Error("DATV Reporter can't connect - check your settings");
                }
            }
            videoSource.UpdateFrequencyPresets(stored_frequencies);
            videoSource.Start();

            splitContainer1.SplitterDistance = _settings.gui_main_splitter_position;

            //toolstripConnectToggle.Text = "Disconnect Source";
            toolstripConnectToggle.Visible = false;

            // hide/show panels
            TogglePropertiesPanel(_settings.hide_properties);
            ToggleExtraToolPanel(_settings.hide_ExtraTool);

            return true;
        }

        private void btnSourceConnect_Click(object sender, EventArgs e)
        {
            source_connected = ConnectSelectedSource();
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

        private void linkBatcSpectrumSettings_Click(object sender, EventArgs e)
        {
            // BATC tune settings
            BATCSpectrumSettings batc_settings = new BATCSpectrumSettings();
            SettingsManager<BATCSpectrumSettings> batc_settingsManager = new SettingsManager<BATCSpectrumSettings>("spectrumSettings");
            batc_settings = batc_settingsManager.LoadSettings(batc_settings);

            BATCSpectrumSettingsForm batc_settings_form = new BATCSpectrumSettingsForm(ref batc_settings);

            if (batc_settings_form.ShowDialog() == DialogResult.OK)
            {
                batc_settingsManager.SaveSettings(batc_settings);
            }
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

        private void menuManageFrequencyPresets_Click(object sender, EventArgs e)
        {
            frequencyManagerForm freqManager = new frequencyManagerForm(stored_frequencies);

            freqManager.ShowDialog();

            frequenciesManager.SaveSettings(stored_frequencies);

            videoSource?.UpdateFrequencyPresets(stored_frequencies);
        }

        private void TogglePropertiesPanel(bool hide)
        {
            if (hide)
            {
                splitContainer1.Panel1.Hide();
                splitContainer1.Panel1Collapsed = true;
                properties_hidden = true;
            }
            else
            {
                splitContainer1.Panel1.Show();
                splitContainer1.Panel1Collapsed = false;
                properties_hidden = false;
            }
        }

        private void ToggleExtraToolPanel(bool hide)
        {
            if (hide)
            {
                splitContainer2.Panel2.Hide();
                splitContainer2.Panel2Collapsed = true;
                ExtraTool_hidden = true;
            }
            else
            {
                splitContainer2.Panel2.Show();
                splitContainer2.Panel2Collapsed = false;
                ExtraTool_hidden = false;
            }
        }

        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == 0X0100 && (Keys)m.WParam.ToInt32() == Keys.P && ModifierKeys == Keys.Control)
            {
                TogglePropertiesPanel(!properties_hidden);
                _settings.hide_properties = properties_hidden;
                return true;
            }
            if (m.Msg == 0X0100 && (Keys)m.WParam.ToInt32() == Keys.E && ModifierKeys == Keys.Control)
            {
                ToggleExtraToolPanel(!ExtraTool_hidden);
                _settings.hide_ExtraTool = ExtraTool_hidden;
                return true;
            }

            return false;
        }

        private void toolstripConnectToggle_Click(object sender, EventArgs e)
        {
            if (!source_connected)
            {
                source_connected = ConnectSelectedSource();
            }
        }

        private void linkDATVReporterSettings_Click(object sender, EventArgs e)
        {
            datv_reporter.ShowSettings();
        }

        private void checkDATVReporter_CheckedChanged(object sender, EventArgs e)
        {
            _settings.enable_datvreporter_checkbox = checkDATVReporter.Checked;
        }

        private void ExtraToolsTab_DrawItem(object sender, DrawItemEventArgs e)
        {

        }

        private void LinkDatvReportMoreInfo_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.zr6tg.co.za/opentuner-datv-reporter/");
        }
    }


}
