using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using Serilog;

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

            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\settings"))
            {
                Debug("Settings Directory doesn't exist, creating...");
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "\\settings");
                Debug("Settings Directory: " + AppDomain.CurrentDomain.BaseDirectory + "\\settings");
            }

            _filename_base = AppDomain.CurrentDomain.BaseDirectory + "\\settings\\" + _settings_name + ".json";
            Debug("Setting File: " +  _filename_base);
        }

        private void Debug(string message)
        {
            Log.Information(_settings_name + " : " + message);
        }

        public T LoadSettings(object _settings_reference)
        {
            
            if (!File.Exists(_filename_base))
            {
                Debug("Base settings doesn't exist, creating new one with defaults");
                if (SaveSettings(_settings_reference))
                    Debug("Settings Saved");
            }

            Debug("Loading Settings...");
            string json_input = File.ReadAllText(_filename_base);
            
            Debug(json_input);

            var settings_object = (T)JsonConvert.DeserializeObject(json_input, typeof(T));

            if (settings_object == null)
            {
                Debug("File possibly faulty - returning defaults");
                return (T)_settings_reference;
            }

            return settings_object;
        }

        public bool SaveSettings(object _settings_reference)
        {
            try
            {
                string json_output = JsonConvert.SerializeObject(_settings_reference, Formatting.Indented);
                Debug("Saving...");
                Debug(json_output);
                File.WriteAllText(_filename_base, json_output);
                
            }
            catch ( Exception Ex )
            {
                Debug("Error saving settings: " + Ex.Message);
                return false;
            }

            return true;
        }
    }
}
