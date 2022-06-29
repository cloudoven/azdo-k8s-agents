

using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using System;
using System.Collections.Generic;

namespace AgentController.Supports
{
    public class InstrumentationClient
    {
        private readonly bool _instrumentationEnabled;
        private readonly TelemetryClient _client;
        private readonly bool _enableConsoleLogs;

        public InstrumentationClient(string connectionString, bool disableConsoleLogs = false)
        {
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                this._client = new TelemetryClient(new TelemetryConfiguration()
                {
                    ConnectionString = connectionString
                });
                _instrumentationEnabled = true;
            }
            this._enableConsoleLogs = !disableConsoleLogs;
        }

        public void TrackError(Exception exception)
        {
            if (_instrumentationEnabled)
            {
                this._client.TrackException(exception);
            }

            if (_enableConsoleLogs)
            {
                Console.BackgroundColor = ConsoleColor.Yellow;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(exception.Message);
                Console.ResetColor();
            }
        }

        public void TrackEvent(string message, IDictionary<string, string> properties, bool enableConsole = true)
        {
            if (_instrumentationEnabled) 
            {
                this._client.TrackEvent(message, properties);
            }
            
            if(_enableConsoleLogs && enableConsole)
            {
                Console.WriteLine(message);
                if (properties != null)
                {
                    foreach(var kv in properties)
                    {
                        Console.WriteLine($"\t #{kv.Key} = {kv.Value}");
                    } 
                }
            }
        }
    }
}
