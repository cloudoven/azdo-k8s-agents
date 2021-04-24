﻿
using AgentController.AzDO.Supports;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AgentController.AzDO
{
    public class AgentService : RestServiceBase
    {
        public AgentService(string adoUrl, string pat)
            : base(adoUrl, pat)
        {
            
        }

        public async Task<List<Job>> ListJobRequestsUIAsync(int poolId)
        {
            var option = new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            option.Converters.Add(new UnixEpochDateConverter());
            option.Converters.Add(new JsonStringEnumConverter());

            var path = $"{OrgName}/_settings/agentpools?poolId={poolId}&__rt=fps&__ver=2";
            var response = await Http.GetRestAsync<UndocumentedJobs>(path, option);
            var jobs = response.Fps.DataProviders.Data.MsVssBuildWebAgentJobsDataProvider.Jobs;
            return jobs;
        }

        public async Task<bool> DeleteAgentAsync(int poolId, int agentId)
        {   
            return await Http.DeleteRestAsync($"{OrgName}/_apis/distributedtask/pools/{poolId}/agents/{agentId}?api-version=6.1-preview.1");            
        }

        public async Task<Pool> GetPoolByNameAsync(string poolName)
        {
            var path = $"{OrgName}/_apis/distributedtask/pools?poolName={poolName}&api-version=6.1-preview.1";
            var response = await Http.GetRestAsync<PoolCollection>(path);
            if(response != null && response.Value != null && response.Value.Count > 0)
            {
                return response.Value.FirstOrDefault();
            }
            return default(Pool);
        }

        public async Task<ReservedAgent> GetAgentByNameAsync(int poolId, string agentName)
        {            
            var path = $"{OrgName}/_apis/distributedtask/pools/{poolId}/agents?agentName={agentName}&api-version=6.1-preview.1";
            var response = await Http.GetRestAsync<AgentCollection>(path);
            if (response != null && response.Value != null && response.Value.Count > 0)
            {
                return response.Value.FirstOrDefault();
            }
            return default(ReservedAgent);
        }
    }
}