using CoreAudio;
using opentuner.ExtraFeatures.MqttClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using Newtonsoft.Json.Linq;
using opentuner.Utilities;
using System.Runtime.InteropServices;

namespace opentuner.Transmit
{
    public class F5OEOPlutoControl
    {
        public enum PlutoConfigCommand
        {
            SETHARDWAREMODE,
            SETFREQUENCY,
        }

        private MqttManager _mqtt_client;

        private string _detected_callsign = "";
        private string _pluto_cmd_path = "";

        public bool _callsign_configured = false;

        private PlutoControl _pluto_control_window;

        private delegate void UpdateLabelDelegate(Label Lbl, Object obj);
        private delegate void UpdateToolStripLabelDelegate(StatusStrip Lbl, string Item, Object obj);
        private delegate void UpdateComboSelectedIndexDelegate(ComboBox Combo, int SelectedIndex);

        // properties
        private DynamicPropertyGroup _hardware_properties;
        private DynamicPropertyGroup _modulator_properties;
        private DynamicPropertyGroup _input_transport_properties;

        // context menu strip
        ContextMenuStrip _genericContextStrip;

        public F5OEOPlutoControl(MqttManager MqttClient)
        {
            _mqtt_client = MqttClient;
            _mqtt_client.OnMqttMessageReceived += _mqtt_client_OnMqttMessageReceived;

            // setup dynamic groups
            _genericContextStrip = new ContextMenuStrip();
            _genericContextStrip.Opening += _genericContextStrip_Opening;

            _pluto_control_window = new PlutoControl();

            _input_transport_properties = new DynamicPropertyGroup("Input Transport", _pluto_control_window.overViewPage);
            _input_transport_properties.AddItem("ts_mode", "TS Mode");
            _input_transport_properties.AddItem("ts_source", "TS Source");

            _modulator_properties = new DynamicPropertyGroup("Modulator", _pluto_control_window.overViewPage);
            _modulator_properties.AddItem("mod_transmitting", "Transmitting");
            _modulator_properties.AddItem("mod_frequency", "Frequency");
            _modulator_properties.AddItem("mod_gain", "Gain");
            _modulator_properties.AddItem("mod_sr", "Symbol Rate");
            _modulator_properties.AddItem("mod_pilots", "Pilots");
            _modulator_properties.AddItem("mod_frame", "Frame");
            _modulator_properties.AddItem("mod_fec", "FEC");
            _modulator_properties.AddItem("mod_fec_mode", "FEC Mode");
            _modulator_properties.AddItem("mod_mod", "Mod");
            //_pluto_control_window.Show();

            _hardware_properties = new DynamicPropertyGroup("Hardware", _pluto_control_window.overViewPage);
            _hardware_properties.AddItem("hw_mode", "Mode", _genericContextStrip);
            _hardware_properties.AddItem("hw_temperature", "Temperature");
            _hardware_properties.AddItem("callsign", "Callsign");


        }

        private ToolStripMenuItem ConfigureMenuItem(string Text, PlutoConfigCommand command, int option)
        {
            ToolStripMenuItem item = new ToolStripMenuItem(Text);
            item.Click += (sender, e) =>
            {
                _pluto_control_window_OnConfigChange(command, option);
            };

            return item;
        }

        private ToolStripTextBox ConfigureTextBox(PlutoConfigCommand command, string value)
        {
            ToolStripTextBox item = new ToolStripTextBox();
            item.Text = value;
            item.KeyUp += (sender, e) =>
            {
                if (e.KeyData == Keys.Enter)
                {
                    Console.WriteLine("Changed");
                    int option_value = 0;

                    if (Int32.TryParse(((ToolStripTextBox)sender).Text, out option_value))
                    {
                        _pluto_control_window_OnConfigChange(command, option_value);
                        e.Handled = true;
                        _genericContextStrip.Close();
                    }

                }
            };

            return item;
        }
        private void _genericContextStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ContextMenuStrip contextMenuStrip = (ContextMenuStrip)sender;
            Console.WriteLine(contextMenuStrip.SourceControl.Name);

            contextMenuStrip.Items.Clear();

            switch (contextMenuStrip.SourceControl.Name)
            {
                // change mode
                case "hw_mode":
                    contextMenuStrip.Items.Add(ConfigureMenuItem("Passthrough", PlutoConfigCommand.SETHARDWAREMODE, 0));
                    contextMenuStrip.Items.Add(ConfigureMenuItem("DVBS2 TS", PlutoConfigCommand.SETHARDWAREMODE, 1));
                    contextMenuStrip.Items.Add(ConfigureMenuItem("DVBS2 GSE", PlutoConfigCommand.SETHARDWAREMODE, 2));
                    contextMenuStrip.Items.Add(ConfigureMenuItem("Test Tone", PlutoConfigCommand.SETHARDWAREMODE, 3));
                    break;

                // change frequency
                case "txtFrequency":
                    // probably a better way to do this
                    contextMenuStrip.Items.Add(ConfigureTextBox(PlutoConfigCommand.SETFREQUENCY, contextMenuStrip.SourceControl.Text.Replace(".", "").Substring(0, contextMenuStrip.SourceControl.Text.Length)));
                    break;
            }

        }

        private void UpdateComboSelectedIndex(ComboBox Combo, int SelectedIndex)
        {

            if (Combo == null)
                return;

            if (Combo.InvokeRequired)
            {
                UpdateComboSelectedIndexDelegate ulb = new UpdateComboSelectedIndexDelegate(UpdateComboSelectedIndex);
                if (Combo != null)
                {
                    Combo?.Invoke(ulb, new object[] { Combo, SelectedIndex });
                }
            }
            else
            {
                Combo.SelectedIndex = SelectedIndex;
            }

        }


        private void UpdateLabel(Label Lbl, Object obj)
        {

            if (Lbl == null)
                return;

            if (Lbl.InvokeRequired)
            {
                UpdateLabelDelegate ulb = new UpdateLabelDelegate(UpdateLabel);
                if (Lbl != null)
                {
                    Lbl?.Invoke(ulb, new object[] { Lbl, obj });
                }
            }
            else
            {
                Lbl.Text = obj.ToString();
            }

        }

        private void UpdateToolStripLabel(StatusStrip Lbl, string Item, Object obj)
        {

            if (Lbl == null)
                return;

            if (Lbl.InvokeRequired)
            {
                UpdateToolStripLabelDelegate ulb = new UpdateToolStripLabelDelegate(UpdateToolStripLabel);
                if (Lbl != null)
                {
                    Lbl?.Invoke(ulb, new object[] { Lbl, Item, obj });
                }
            }
            else
            {
                Lbl.Items[Item].Text = obj.ToString();
            }
        }



        private void _pluto_control_window_OnConfigChange(PlutoConfigCommand command, int option)
        {
            Console.WriteLine("Config Change: " + command.ToString() + " - " + option.ToString());

            switch(command)
            {
                case PlutoConfigCommand.SETHARDWAREMODE: 
                    switch (option)
                    {
                        case 0: ConfigureHardwareMode("pass"); break;
                        case 1: ConfigureHardwareMode("dvbs2-ts"); break;
                        case 2: ConfigureHardwareMode("dvbs2-gse"); break;
                        case 3: ConfigureHardwareMode("test"); break;
                    }
                    break;

                case PlutoConfigCommand.SETFREQUENCY:
                    SetFrequency(option);
                    break;

                default:
                    Console.WriteLine("Unconfigured Command Change - " + command.ToString());
                    break;
            }
        }

        private void _mqtt_client_OnMqttMessageReceived(MqttMessage Message)
        {
            if (Message.Topic.Contains("dt/pluto"))
            {
                string[] parts = Message.Topic.Split('/');

                if (!_callsign_configured)
                {
                    if (parts[2].ToUpper() != "NOCALL")
                    {
                        _callsign_configured = true;
                        _detected_callsign = parts[2];
                        _pluto_cmd_path = "cmd/pluto/" + _detected_callsign;
                        _hardware_properties.UpdateValue("callsign", _detected_callsign.ToUpper());
                    }
                    else
                    {
                        return;
                    }
                }
            }

            //Console.WriteLine(Message.Topic.Substring(("dt/pluto/" + _detected_callsign).Length));

            switch (Message.Topic.Substring(("dt/pluto/" + _detected_callsign).Length))
            {
                case "/system/version":
                    UpdateToolStripLabel(_pluto_control_window.statusStrip, "lblVersion", "Version: " + Message.Message);
                    break;

                case "/tx/frequency":
                    double frequency = 0;
                    double.TryParse(Message.Message, out frequency);
                    _modulator_properties.UpdateValue("mod_frequency", frequency.ToString());
                    break;

                case "/tx/mute":
                    _modulator_properties.UpdateValue("mod_transmitting", (Message.Message == "1" ? "On" : "Off"));
                    break;

                case "/tx/gain":
                    double gain = 0;
                    double.TryParse(Message.Message, out gain);
                    _modulator_properties.UpdateValue("mod_gain", string.Format("{0:N2} db", gain));
                    break;

                case "/tx/dvbs2/tssourcemode":
                    int source_mode = 0;

                    if (int.TryParse(Message.Message, out source_mode))
                    {
                        string source_mode_text = "";

                        switch(source_mode)
                        {
                            case 0: source_mode_text = "UDP";
                                break;
                            case 1: source_mode_text = "File";
                                break;
                            case 2: source_mode_text = "Pattern";
                                break;
                            default:
                                source_mode_text = "Unknown";
                                break;
                        }

                        _input_transport_properties.UpdateValue("ts_mode", source_mode_text);
                    }
                    break;

                case "/tx/dvbs2/tssourceaddress":
                    _input_transport_properties.UpdateValue("ts_source", Message.Message);
                    break;


                case "/tx/stream/mode":

                    string mode_text = "";
                    
                    switch(Message.Message)
                    {
                        case "pass": mode_text = "Passthrough"; break;
                        case "dvbs2-ts": mode_text = "DVBS2 TS"; break;
                        case "dvbs2-gse": mode_text = "DVBS2 GSE"; break;
                        case "test": mode_text = "Test Tone"; break;
                    }

                    _hardware_properties.UpdateValue("hw_mode", mode_text);
                    break;

                case "/tx/dvbs2/fec":
                    _modulator_properties.UpdateValue("mod_fec", Message.Message);
                    break;

                case "/tx/dvbs2/constel":
                    _modulator_properties.UpdateValue("mod_mod", Message.Message.ToUpper());
                    break;

                case "/tx/dvbs2/frame":
                    _modulator_properties.UpdateValue("mod_frame", Message.Message.ToUpper());
                    break;

                case "/tx/dvbs2/pilots":
                    _modulator_properties.UpdateValue("mod_pilots", (Message.Message == "1" ? "On" : "Off"));
                    break;

                case "/tx/dvbs2/sr":
                    _modulator_properties.UpdateValue("mod_sr", Message.Message);
                    break;

                case "/tx/dvbs2/fecmode":
                    _modulator_properties.UpdateValue("mod_fec_mode", Message.Message.ToUpper());
                    break;

                case "/temperature_ad":
                    _hardware_properties.UpdateValue("hw_temperature", Message.Message);
                    break;

                default:
                    break;
            }
        }

        // *********************** PLUTO Commands *******************************************************


        public void SendMqttCommand(string topic, string value)
        {
            Task.Run(async () =>
            {
                await _mqtt_client.SendMqttCommand(topic, value);
            });
        }

        public void ConfigureHardwareMode(string mode)
        {
            SendMqttCommand(_pluto_cmd_path + "/tx/stream/mode", mode);
        }

        public void SetFrequency(int frequency)
        {
            SendMqttCommand(_pluto_cmd_path + "/tx/frequency", frequency.ToString());
        }

        // doesnt require a reboot command - will reboot automatically
        // TODO: mqtt needs to automatically reconnect
        public void ConfigureCallsignAndReboot(string Callsign)
        {
            SendMqttCommand("cmd/pluto/call", Callsign);
        }

        // doesn't seem to work
        public void Reboot()
        {
            SendMqttCommand("system/reboot", "reboot");
        }

        public void Close()
        {
            _pluto_control_window.Close();
        }
        
    }
}
