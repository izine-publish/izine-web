using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.IO;

/// <summary>
/// Summary description for LINQLogger
/// </summary>
namespace LINQ2Log4Net
{
    public class LINQLogger : TextWriter
    {
        //The ILogger Instance
        private ILogger _logger;
        public ILogger Logger
        {
            get { return _logger; }
            set { _logger = value; }
        }
        //The Log level enum
        private LogLevelType _logLevelType;
        public LogLevelType LogLevel
        {
            get { return _logLevelType; }
            set { _logLevelType = value; }
        }

        private Encoding _encoding;
        public override Encoding Encoding
        {
            get
            {
                if (_encoding == null)
                {
                    _encoding = new UnicodeEncoding(false, false);
                }
                return _encoding;
            }
        }

        public LINQLogger()
        {

        }

        public LINQLogger(ILogger logger, LogLevelType logLevel)
        {
            _logger = logger;
            _logLevelType = logLevel;
        }

        public override void Write(string value)
        {
            switch (_logLevelType)
            {
                case LogLevelType.Fatal: _logger.Fatal(value); break;
                case LogLevelType.Error: _logger.Error(value); break;
                case LogLevelType.Warn: _logger.Warn(value); break;
                case LogLevelType.Info: _logger.Info(value); break;
                case LogLevelType.Debug: _logger.Debug(value); break;
            }
        }

        public override void Write(char[] buffer, int index, int count)
        {
            Write(new string(buffer, index, count));
        }
    }

    public enum LogLevelType
    {
        Fatal,
        Error,
        Warn,
        Info,
        Debug
    }
}