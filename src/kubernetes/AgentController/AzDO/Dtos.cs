

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AgentController.AzDO
{
    #region Pipelines
    public class Pipeline
    {
        public string Url { get; set; }
        public int Id { get; set; }
        public int Revision { get; set; }
        public string Name { get; set; }
        public string Folder { get; set; }
    }

    public class PipelineCollection
    {
        public int Count { get; set; }
        public List<Pipeline> Value { get; set; }
    }
    #endregion

    #region POOL
    public class Pool
    {
        [JsonPropertyName("createdOn")]
        public DateTime CreatedOn { get; set; }

        [JsonPropertyName("autoProvision")]
        public bool AutoProvision { get; set; }

        [JsonPropertyName("autoUpdate")]
        public bool AutoUpdate { get; set; }

        [JsonPropertyName("autoSize")]
        public bool AutoSize { get; set; }

        [JsonPropertyName("targetSize")]
        public int? TargetSize { get; set; }

        [JsonPropertyName("agentCloudId")]
        public int? AgentCloudId { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("scope")]
        public string Scope { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("isHosted")]
        public bool IsHosted { get; set; }

        [JsonPropertyName("poolType")]
        public string PoolType { get; set; }

        [JsonPropertyName("size")]
        public int Size { get; set; }

        [JsonPropertyName("isLegacy")]
        public bool IsLegacy { get; set; }

        [JsonPropertyName("options")]
        public string Options { get; set; }

        public override string ToString()
        {
            return $"{Name} - {PoolType}; Hosted={this.IsHosted}; Agent Cloud ID: {AgentCloudId}";
        }
    }

    public class PoolCollection
    {
        public int Count { get; set; }
        public List<Pool> Value { get; set; }
    }


    #endregion

    #region Agents
    public class AgentCollection
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("value")]
        public List<ReservedAgent> Value { get; set; }
    }
    #endregion


    #region JOBS
    
    public class JobRequestData
    {
        [JsonPropertyName("ParallelismTag")]
        public string ParallelismTag { get; set; }

        [JsonPropertyName("IsScheduledKey")]
        public string IsScheduledKey { get; set; }
    }

    public class JobDefinition
    {

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }

    public class MatchedAgent
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("version")]
        public string Version { get; set; }

        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("provisioningState")]
        public string ProvisioningState { get; set; }
    }

    public class JobRequestOwner
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }

    public class ReservedAgent
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("version")]
        public string Version { get; set; }

        [JsonPropertyName("osDescription")]
        public string OsDescription { get; set; }

        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("provisioningState")]
        public string ProvisioningState { get; set; }

        [JsonPropertyName("accessPoint")]
        public string AccessPoint { get; set; }
    }

    public class JobRequestCollection
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("value")]
        public List<JobRequest> Jobs { get; set; }
    }


    public class JobRequest
    {
        [JsonPropertyName("requestId")]
        public int RequestId { get; set; }

        [JsonPropertyName("queueTime")]
        public DateTime QueueTime { get; set; }

        [JsonPropertyName("serviceOwner")]
        public string ServiceOwner { get; set; }

        [JsonPropertyName("hostId")]
        public string HostId { get; set; }

        [JsonPropertyName("scopeId")]
        public string ScopeId { get; set; }

        [JsonPropertyName("planType")]
        public string PlanType { get; set; }

        [JsonPropertyName("planId")]
        public string PlanId { get; set; }

        [JsonPropertyName("jobId")]
        public string JobId { get; set; }

        [JsonPropertyName("demands")]
        public List<string> Demands { get; set; }

        [JsonPropertyName("matchedAgents")]
        public List<MatchedAgent> MatchedAgents { get; set; }

        [JsonPropertyName("definition")]
        public JobDefinition Definition { get; set; }

        [JsonPropertyName("owner")]
        public JobRequestOwner Owner { get; set; }

        [JsonPropertyName("data")]
        public JobRequestData Data { get; set; }

        [JsonPropertyName("poolId")]
        public int PoolId { get; set; }

        [JsonPropertyName("orchestrationId")]
        public string OrchestrationId { get; set; }

        [JsonPropertyName("matchesAllAgentsInPool")]
        public bool MatchesAllAgentsInPool { get; set; }

        [JsonPropertyName("priority")]
        public int Priority { get; set; }

        [JsonPropertyName("assignTime")]
        public DateTime? AssignTime { get; set; }

        [JsonPropertyName("receiveTime")]
        public DateTime? ReceiveTime { get; set; }

        [JsonPropertyName("lockedUntil")]
        public DateTime? LockedUntil { get; set; }

        [JsonPropertyName("reservedAgent")]
        public ReservedAgent ReservedAgent { get; set; }

        public override string ToString()
        {
            return $"ID: {JobId}; QT:{QueueTime}; AT:{this.AssignTime}; RT:{this.ReceiveTime};";
        }
    }

    #endregion
}
