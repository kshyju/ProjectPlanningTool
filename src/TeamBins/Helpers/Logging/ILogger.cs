using log4net;
using log4net.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TechiesWeb.TeamBibs.Helpers.Logging
{
    public interface ILogger
    {
        void Debug(object message);
        void Warn(object message);
        void Error(object message);
        void Error(object message, Exception exception);
        void Warn(object message, Exception exception);
        void Debug(object message, Exception exception);
    }
}
