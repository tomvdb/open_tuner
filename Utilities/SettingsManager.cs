using Newtonsoft.Json;
using Serilog;
using System;
using System.IO;

namespace opentuner.Utilities
{
    public class SettingsManager<T>
    {
        private string _settings_name;
        private string _filename_base;

        public SettingsManager(String SettingsGroupName) 
        {
            _settings_name = SettingsGroupName;

            // check if settings directory exists ?

            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "settings"))
            {
                Log.Information("Settings Directory " + AppDomain.CurrentDomain.BaseDirectory + " doesn't exist, creating...");
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "settings");
                Log.Debug("SettingsManager.SettingsManager: " + _settings_name + " : Settings Directory: " + AppDomain.CurrentDomain.BaseDirectory + "settings");
            }

            _filename_base = AppDomain.CurrentDomain.BaseDirectory + "settings\\" + _settings_name + ".json";
            Log.Debug("SettingsManager.SettingsManager: " + _settings_name + " : Setting File: " + _filename_base);
        }

        public T LoadSettings(object _settings_reference)
        {
            if (!File.Exists(_filename_base))
            {
                Log.Information("Settings file " + _settings_name + ".json doesn't exist, creating new one with defaults");
                if (!SaveSettings(_settings_reference))
                {
                    Log.Error("SettingsManager.LoadSettings: " + _settings_name + ".json : creation of settings file failed");
                }
                else
                {
                    Log.Debug("SettingsManager.LoadSettings: " + _settings_name + ".json : Settings saved");
                }
            }

            Log.Information("Loading " + _settings_name + ".json");
            string json_input = File.ReadAllText(_filename_base);
            Log.Debug("Data: " + json_input);
            T settings_object = default(T);

            try
            {
                settings_object = (T)JsonConvert.DeserializeObject(json_input, typeof(T));
            }

            catch (Exception ex)
            {
                Log.Error(ex, "SettingsManager.LoadSettings: " + _settings_name + ".json : Execption rised. File possibly faulty - returning defaults");
                Log.Error("Data: " + json_input);
                return (T)_settings_reference;
            }

            if (settings_object == null)
            {
                Log.Error("SettingsManager.LoadSettings: " + _settings_name + ".json : File possibly faulty - returning defaults");
                Log.Error("Data: " + json_input);
                return (T)_settings_reference;
            }
            return settings_object;
        }

        public bool SaveSettings(object _settings_reference)
        {
            try
            {
                string json_output = JsonConvert.SerializeObject(_settings_reference, Formatting.Indented);
                Log.Information("Saving " + _settings_name + ".json");
                Log.Debug("Data: " + json_output);
                File.WriteAllText(_filename_base, json_output);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "SettingsManager.SaveSettings: " + _settings_name + ".json : Execption rised. Error saving settings");
                return false;
            }
            return true;
        }
    }
}
