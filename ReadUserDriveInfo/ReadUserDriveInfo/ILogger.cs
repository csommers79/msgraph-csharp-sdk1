using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Logging
{
    public interface ILogger
    {
        void Info(string message);
        void Error(string message);


    }

    public abstract class BaseLogger
    {
        protected bool enableLog { get; set; }
        public BaseLogger(bool enableLog)
        {
            this.enableLog = enableLog;
        }

    }

    public class FileLogger : BaseLogger, ILogger
    {
        private readonly string _filename;
        public FileLogger(bool enableLog, string logFilePath) : base(enableLog)
        {
            _filename = string.Format(@"{0}\{1}", logFilePath, "OneDriveUploadDownload.txt");
        }


        public void Error(string message)
        {
            if (!enableLog) return;
            File.AppendAllText(_filename, "\r\n - Error: " + message);
        }

        public void Info(string message)
        {
            if (!enableLog) return;
            File.AppendAllText(_filename, "\r\n - Info: " + message);
        }
    }
}
