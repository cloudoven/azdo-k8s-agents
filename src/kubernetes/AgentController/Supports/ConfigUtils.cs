

using System;
using System.Diagnostics;

namespace AgentController.Supports
{
    public class ConfigUtils
    {
        private const string ENV_AZDO_ORG_URI = "AZDO_ORG_URI";
        private const string ENV_AZDO_PAT = "AZDO_TOKEN";
        private const string ENV_AZDO_POOLNAME = "AZDO_POOLNAME";
        private const string ENV_TARGET_NAMESPACE = "TARGET_NAMESPACE";
        private const string ENV_STANDBY = "STANDBY_AGENT_COUNT";
        private const string ENV_MAX_AGENT_COUNT = "MAX_AGENT_COUNT";
        private const string ENV_LOAD_LOCAL_K8S_CONFIG = "LOAD_LOCAL_K8S_CONFIG";
        private const string ENV_APPINSIGHT_CONN_STR = "APPINSIGHT_CONN_STR";
        private const string ENV_DISABLE_CONSOLE_LOGS = "DISABLE_CONSOLE_LOGS";

        public record Config(
            string OrgUri, 
            string Pat, 
            string PoolName, 
            string TargetNamespace, 
            int StandBy, 
            int MaxAgentsCount, 
            bool ClusterMode,
            string AppInsightConnectionString,
            bool DisableConsoleLogs);


        public static Config Get()
        {
            var orgUri = ReadEnvironmentVar(ENV_AZDO_ORG_URI, string.Empty, true);
            var pat = ReadEnvironmentVar(ENV_AZDO_PAT, string.Empty, true);
            var poolName = ReadEnvironmentVar(ENV_AZDO_POOLNAME, string.Empty, true);
            var targetNamespace = ReadEnvironmentVar(ENV_TARGET_NAMESPACE, "default", false);
            var standBy = Convert.ToInt32(ReadEnvironmentVar(ENV_STANDBY, "2", false));
            var maxLimit = Convert.ToInt32(ReadEnvironmentVar(ENV_MAX_AGENT_COUNT, "25", false));
            var clusterMode = string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(ENV_LOAD_LOCAL_K8S_CONFIG));
            var appInsightConnectionString = ReadEnvironmentVar(ENV_APPINSIGHT_CONN_STR, string.Empty, false);

            var disableConsoleLogsString = ReadEnvironmentVar(ENV_DISABLE_CONSOLE_LOGS, string.Empty, false);
            var disableConsoleError =
                disableConsoleLogsString.Equals("yes", StringComparison.OrdinalIgnoreCase) ||
                disableConsoleLogsString.Equals("true", StringComparison.OrdinalIgnoreCase);

            return new Config(orgUri, pat, poolName, 
                targetNamespace, standBy, maxLimit, 
                clusterMode, appInsightConnectionString, 
                disableConsoleError);
        }

        private static string ReadEnvironmentVar(string key, string defaultValue, 
            bool failWhenMissing)
        {            
            var value = Environment.GetEnvironmentVariable(key);

            if(string.IsNullOrWhiteSpace(value))
            {
                if(failWhenMissing)
                {
                    Console.WriteLine($"Expected Envrionment key '{key}' was not set.");
                    throw new InvalidProgramException($"Expected Envrionment key '{key}' was not set.");
                }
                else
                {
                    value = defaultValue;
                }
            }
            return value;
        }
    }
}