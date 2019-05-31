using System;
using System.IO;

namespace VkCelebrationScheduler.Logging
{
    public class SimpleFileLogger : ISimpleLogger
    {
        private const string FILE_EXT = ".log";
        private const string FILE_DIR_NAME = "logs";

        private readonly string _datetimeFormat;
        private readonly string _fileDirPath;

        public SimpleFileLogger(string fileDirPath)
        {
            _fileDirPath = fileDirPath;
            _datetimeFormat = "yyyy-MM-dd HH:mm:ss.fff";

            Directory.CreateDirectory(fileDirPath + "\\" + FILE_DIR_NAME);
        }

        public void WriteException(Exception ex)
        {
            WriteLine(DateTime.Now.ToString(_datetimeFormat) + " [ERROR] " + ex.Message + "\n" + ex.StackTrace);
        }

        public void WriteLine(string text)
        {
            var logFilename = $"{_fileDirPath}\\{FILE_DIR_NAME}"
                + $"\\{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}"
                + $"_{DateTime.Now.ToString("yyyy-MM-dd")}{FILE_EXT}";

            using (StreamWriter writer = new StreamWriter(logFilename, true, System.Text.Encoding.UTF8))
            {
                if (!string.IsNullOrEmpty(text))
                {
                    writer.WriteLine(text);
                }
            }
        }
    }
}
