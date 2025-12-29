using System;
using System.Globalization;
using WebSocketSharp;
using Newtonsoft.Json;
using Serilog;
using opentuner.Utilities;
using System.Timers;

namespace opentuner.ExtraFeatures.DATVReporter
{

    public class DATVReporter
    {
        private DATVReporterSettingsForm _settings_form;
        private DATVReporterSettings _datv_reporter_settings;
        private SettingsManager<DATVReporterSettings> _settings_manager;

        private WebSocket _websocket;

        public bool Connected = false;

        private string _last_callsign = "";
        private TimeSpan _last_callsign_threshold = TimeSpan.FromSeconds(30);
        private DateTime _last_callsign_timestamp = DateTime.MinValue;

        private TimeSpan _last_send_threshold = TimeSpan.FromSeconds(10);
        private DateTime _last_send_timestamp = DateTime.MinValue;

        private static Timer _timer;

        public void ShowSettings()
        {
            _settings_form = new DATVReporterSettingsForm();
            _settings_form.txtCallsign.Text = _datv_reporter_settings.callsign;
            _settings_form.txtGridLocator.Text = _datv_reporter_settings.grid_locator;
            _settings_form.txtServiceUrl.Text = _datv_reporter_settings.service_url;

            if (_settings_form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _datv_reporter_settings.callsign = _settings_form.txtCallsign.Text.ToUpper();
                _datv_reporter_settings.grid_locator = _settings_form.txtGridLocator.Text;
                _datv_reporter_settings.service_url = _settings_form.txtServiceUrl.Text;

                _settings_manager.SaveSettings(_datv_reporter_settings);
            }

        }

        public bool AllowedSend()
        {
            var current = DateTime.Now;

            TimeSpan time_difference = current - _last_send_timestamp;

            if (time_difference > _last_send_threshold)
                return true;

            return false;
        }

        public bool AllowedSameCallsign()
        {
            var current = DateTime.Now;

            TimeSpan time_difference = current - _last_callsign_timestamp;

            if (time_difference > _last_callsign_threshold)
                return true;

            return false;
        }

        public DATVReporter()
        {
            // datv reporter settings
            _datv_reporter_settings = new DATVReporterSettings();
            _settings_manager = new SettingsManager<DATVReporterSettings>("datvreporter_settings");
            _datv_reporter_settings = (_settings_manager.LoadSettings(_datv_reporter_settings));

            _timer = new Timer(10000);

            _timer.Elapsed += _timer_Elapsed;
            _timer.Enabled = false;
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if ( !Connected )
            {
                Log.Warning("DATV Reporter : Attempt Reconnect");
                Connect();
            }
        }

        private void Debug(string msg)
        {
            Console.WriteLine("datv-spotter: " + msg);
        }

        public bool Connect()
        {
            if (_datv_reporter_settings.callsign.IsNullOrEmpty())
            {
                Log.Error("DATV Reporters: Callsign is Empty");
                return false;
            }

            if (_datv_reporter_settings.grid_locator.IsNullOrEmpty())
            {
                Log.Error("DATV Reporter: Grid locator is Empty");
                return false;
            }

            if (_datv_reporter_settings.service_url.IsNullOrEmpty())
            {
                Log.Error("DATV Reporter: Service URL is Empty");
                return false;
            }

            string url = _datv_reporter_settings.service_url;

            if (!Connected)
            {
                Debug("Connecting: " + url);

                _websocket = new WebSocket(url);
                // _websocket.Log.Level = LogLevel.Trace;
                _websocket.SslConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12;
                _websocket.OnClose += _websocket_OnClose;
                _websocket.OnMessage += _websocket_OnMessage;
                _websocket.OnOpen += _websocket_OnOpen;
                _websocket.OnError += _websocket_OnError;

                _websocket.ConnectAsync();

                return true;
            }

            Log.Warning("DATV Reporter: Already Connected");
            return false;
        }

        public void Close()
        {
            _timer?.Stop();
            _websocket?.Close();
        }

        public bool SendISawMessage(ISawMessage message)
        {
            if (!Connected)
            {
                return false;
            }

            if (message.target_callsign == null)
                return false;

            if (message.target_callsign.Length == 0)
                return false;

            // if we have sent anything in the past 5 seconds then lets not send again
            if (!AllowedSend())
                return false;

            // if we have already sent the callsign in the last 30 seconds then lets not send again
            if (_last_callsign == message.target_callsign)
            {
                if (!AllowedSameCallsign())
                    return false;
            }

            var compileTime = new DateTime(Builtin.CompileTime, DateTimeKind.Utc);
            DateTimeFormatInfo usDateFormat = new CultureInfo("en-US", false).DateTimeFormat;
            string compileTime_usFormat = compileTime.ToString("u", usDateFormat);

            message.observer_callsign = _datv_reporter_settings.callsign;
            message.observer_gridlocator = _datv_reporter_settings.grid_locator;
            message.application = "OpenTuner";
            message.application_version = GlobalDefines.Version + " - " + compileTime_usFormat.Trim();

            string json_output = JsonConvert.SerializeObject(message, Formatting.Indented);

            Log.Information(json_output);

            _last_callsign = message.target_callsign;
            _last_send_timestamp = DateTime.Now;
            _last_callsign_timestamp = DateTime.Now;

            _websocket.Send(json_output);

            return true;
        }

        private void _websocket_OnError(object sender, ErrorEventArgs e)
        {
            Debug("Error: " + e.Message);
        }

        private void _websocket_OnOpen(object sender, EventArgs e)
        {
            Debug("Connected ");
            Connected = true;
            _timer.Enabled = true; // stay alive timer, only start when connected properly first time
        }

        private void _websocket_OnMessage(object sender, MessageEventArgs e)
        {
            Debug("Message Received");
        }

        private void _websocket_OnClose(object sender, CloseEventArgs e)
        {
            Log.Warning("DATV Reporter Disconnected");
            Connected = false;
        }
    }
}