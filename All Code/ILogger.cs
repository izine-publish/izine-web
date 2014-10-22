using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LINQ2Log4Net
{
    // An interface for loggers.
    public interface ILogger
    {
        // This method creates a logger for specified class
        void CreateLogger(Type loggerType);

        // Indexer for setting properties for logging
        string this[string propertyName]
        {
            set;
        }

        //These methods log in various logging levels
        void Debug(string message);
        void Fatal(string message);
        void Info(string message);
        void Error(string message);
        void Warn(string message);
    }
}