using System;
using System.Diagnostics;
using Kernel.Logging;
using Microsoft.Owin.Logging;

namespace SLOOwinMiddleware.Logging
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
            switch(eventType)
            {
                case TraceEventType.Error:
                case TraceEventType.Critical:
                    {
                        Exception inner;
                        this._logProvider.TryLogException(exception, out inner);
                        this._logProvider.LogMessage(formatter(state, exception));
                        break;
                    }
                default:
                    this._logProvider.LogMessage(formatter(state, exception));
                    break;

            }
            return true;
        }
    }
}