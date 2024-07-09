using FlyleafLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Serilog;

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

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("logs\\ot_log_" + DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss") + ".txt")
                .CreateLogger();

            Log.Information("Starting OT");

            try
            {
                Engine.Start(new EngineConfig()
                {
                    FFmpegPath = @"ffmpeg\",
                    FFmpegDevices = false,    // Prevents loading avdevice/avfilter dll files. Enable it only if you plan to use dshow/gdigrab etc.
                                              //LogLevel = LogLevel.Debug,
                                              //LogOutput = ":console",
                                              //LogOutput = @"C:\temp2\ffmpeg.log",

                    /*
                    UIRefresh = true,    // Required for Activity, BufferedDuration, Stats in combination with Config.Player.Stats = true
                    UIRefreshInterval = 250,      // How often (in ms) to notify the UI
                    UICurTimePerSecond = false,     // Whether to notify UI for CurTime only when it's second changed or by UIRefreshInterval
                    */
                });

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Uncaught Exception");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
