using System;
using System.Diagnostics;
using Kernel.Logging;
using Microsoft.Owin.Logging;

namespace SSOOwinMiddleware.Logging
{
    internal class CustomLogger : ILogger
    {
        private readonly ILogProvider _logProvider;
        public CustomLogger(ILogProvider logProvider)
        {
            this._logProvider = logProvider;
        }

        public bool WriteCore(TraceEventType eventType, int eventId, object state, Exception exception, Func<object, Exception, string> formatter)
        {
            return true;
        }
    }
}