using FlyleafLib;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace opentuner
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static LoggingLevelSwitch levelSwitch;

        [STAThread]

        static void Main(string[] args)
        {
            int i = 0;
            int debugLevel = 3; // Warning
            levelSwitch = new LoggingLevelSwitch();

            while (i < args.Length)
            {
                switch (args[i])
                {
                    case "--debuglevel":
                        int new_debug_level = -1;

                        if (int.TryParse(args[i + 1], out new_debug_level))
                        {
                            if (new_debug_level < 6 && new_debug_level >= 0)
                            {
                                debugLevel = new_debug_level;
                            }
                            i += 1;
                        }
                        break;

                    default:
                        break;
                }
                // grab next param
                i += 1;
            }

            switch (debugLevel)
            {
                case 0: // Verbose
                    levelSwitch.MinimumLevel = LogEventLevel.Verbose;
                    break;

                case 1: // Debug
                    levelSwitch.MinimumLevel = LogEventLevel.Debug;
                    break;

                case 2: // Information
                    levelSwitch.MinimumLevel = LogEventLevel.Information;
                    break;

                case 3: // Warning
                    levelSwitch.MinimumLevel = LogEventLevel.Warning;
                    break;

                case 4: // Error
                    levelSwitch.MinimumLevel = LogEventLevel.Error;
                    break;

                case 5: // Fatal
                    levelSwitch.MinimumLevel = LogEventLevel.Fatal;
                    break;

                default:
                    levelSwitch.MinimumLevel = LogEventLevel.Warning;
                    break;
            }

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.ControlledBy(levelSwitch)
                .WriteTo.Console()
                .WriteTo.File("logs\\ot_log_" + DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss") + ".txt")
                .CreateLogger();

            // Always log the starting information
            // swith logging level to Information
            LogEventLevel lastMinimumLevel = levelSwitch.MinimumLevel;
            levelSwitch.MinimumLevel = LogEventLevel.Information;

            Log.Information("Starting OpenTuner");

            // swith logging level back
            levelSwitch.MinimumLevel = lastMinimumLevel;

            string logDirectory = AppDomain.CurrentDomain.BaseDirectory + "logs\\";

            if (Directory.Exists(logDirectory))
            {
                var logFiles = Directory.GetFiles(logDirectory, "*.txt").Select(f => new FileInfo(f)).OrderByDescending(f => f.CreationTime);
                int fileCount = logFiles.Count();
                if (fileCount > 10)
                {
                    i = 0;
                    foreach (var file in logFiles)
                    {
                        if (i > 9)
                        {
                            try
                            {
                                File.Delete(file.FullName);
                                Log.Debug("Log file deleted: " + file.Name);
                            }
                            catch
                            {
                                Log.Warning("Log file for deletion not found: " + file.Name);
                            }
                        }
                        i++;
                    }
                }
            }

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
                Application.Run(new MainForm(args));
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Program.Main: Uncaught Exception");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
