namespace Kernel.Logging
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Security.Principal;

    public class LogEventSourceProvider
    {
        private const string _defaultSource = "_default";
        
        private static IDictionary<string, string> _sources;

        public const string EventLogName = "Flowz";

        static LogEventSourceProvider()
        {
            LogEventSourceProvider._sources = new Dictionary<string, string>();

            LogEventSourceProvider._sources[_defaultSource] = "Flowz Application";
        }

        public static string GetSourceName(string applicationName)
        {
            if(LogEventSourceProvider._sources.ContainsKey(applicationName))
                return LogEventSourceProvider._sources[applicationName];

            if (LogEventSourceProvider.IsRegistered(applicationName))
            {
                LogEventSourceProvider._sources[applicationName] = applicationName;

                return applicationName;
            }

            return LogEventSourceProvider._sources[_defaultSource];
        }

        private static bool IsRegistered(string sourceName)
        {
            var isExists = EventLog.SourceExists(sourceName);

            if (!isExists)
            {
                if(LogEventSourceProvider.IsUserAdministrator())
                {
                    var source = new EventSourceCreationData(sourceName, EventLogName);

                    EventLog.CreateEventSource(source);
                }  
            }

            return isExists;
        }

        private static bool IsUserAdministrator()
        {
            bool isAdmin;
            
            try
            {
                var user = WindowsIdentity.GetCurrent();

                var principal = new WindowsPrincipal(user);

                isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch (UnauthorizedAccessException)
            {
                isAdmin = false;
            }
            catch (Exception)
            {
                isAdmin = false;
            }
            return isAdmin;
        }
    }
}