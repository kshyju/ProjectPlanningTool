using log4net;
using System;

namespace TechiesWeb.TeamBibs.Helpers.Logging
{
    public class Logger : ILogger
    {
        internal ILog log;

        public Logger()
        {   
            log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }
       
        public Logger(Type type)
        {
            log = LogManager.GetLogger(type);
        }
        public Logger(string typeName)
        {           
            log = LogManager.GetLogger(typeName);
        }

        public void Debug(object message)
        {
            if (log.IsDebugEnabled)
            {                
                log.Debug(message);
            }
        }

        public void Error(object message)
        {
            if (log.IsErrorEnabled)
            {
                log.Error(message);
            }
        }

        public void Warn(object message)
        {
            if (log.IsWarnEnabled)
            {
                log.Warn(message);
            }
        }

        public void Warn(object message, Exception exception)
        {
            if (log.IsWarnEnabled)
            {
                log.Warn(message, exception);
            }
        }

        public void Debug(object message, Exception exception)
        {
            if (log.IsDebugEnabled)
            {
                log.Debug(message, exception);
            }
        }

        public void Error(object message, Exception exception)
        {
            if (log.IsErrorEnabled)
            {
                log.Error(message, exception);
            }
        }
    }
}