using FlyleafLib;
using System;
using System.Windows.Forms;

namespace opentuner
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Engine.Start(new EngineConfig()
            {
                FFmpegPath = @"ffmpeg\",
                FFmpegDevices = false,    // Prevents loading avdevice/avfilter dll files. Enable it only if you plan to use dshow/gdigrab etc.
                //LogLevel = LogLevel.Debug,
                //LogOutput = ":console",
                //LogOutput = @"C:\temp\audio_flyleaf_test.log",

                /*
                UIRefresh = true,    // Required for Activity, BufferedDuration, Stats in combination with Config.Player.Stats = true
                UIRefreshInterval = 250,      // How often (in ms) to notify the UI
                UICurTimePerSecond = false,     // Whether to notify UI for CurTime only when it's second changed or by UIRefreshInterval
                */
            });

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new OpenTunerForm());
        }
    }
}
