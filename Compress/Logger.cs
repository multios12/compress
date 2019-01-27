namespace Compress
{
    using System;
    using System.IO;

    public static class Logger
    {
        private static string _logPath;

        private static string LogPath
        {
            get
            {
                if (Logger._logPath == null)
                {
                    string logFolderPath;
                    logFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    logFolderPath = Path.Combine(logFolderPath, "compress");
                    Logger._logPath = Path.Combine(logFolderPath, $"compress-{DateTime.Now.ToString("yyyyMMdd")}.log");

                    if (!Directory.Exists(logFolderPath))
                    {
                        Directory.CreateDirectory(logFolderPath);
                    }
                }

                return _logPath;
            }
        }

        public static void WriteLine(string message) {
            string log = $"{DateTime.Now.ToString("yyyyMMdd HH:mm:ss")} :{message}\r\n";
            File.AppendAllText(Logger.LogPath, log);
        }
    }
}
