

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

    #region JOB requests


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

    public class Definition
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }



    public class Data
    {
        [JsonPropertyName("ParallelismTag")]
        public string ParallelismTag { get; set; }

        [JsonPropertyName("IsScheduledKey")]
        public string IsScheduledKey { get; set; }
    }

    public class AgentSpecification
    {
        [JsonPropertyName("VMImage")]
        public string VMImage { get; set; }

        [JsonPropertyName("vmImage")]
        public string VmImage { get; set; }
    }

    public class JobRequest
    {
        [JsonPropertyName("requestId")]
        public int RequestId { get; set; }

        [JsonPropertyName("queueTime")]
        public DateTime QueueTime { get; set; }

        [JsonPropertyName("assignTime")]
        public DateTime AssignTime { get; set; }

        [JsonPropertyName("receiveTime")]
        public DateTime ReceiveTime { get; set; }

        [JsonPropertyName("finishTime")]
        public DateTime FinishTime { get; set; }

        [JsonPropertyName("result")]
        public string Result { get; set; }

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

        [JsonPropertyName("reservedAgent")]
        public ReservedAgent ReservedAgent { get; set; }

        [JsonPropertyName("definition")]
        public Definition Definition { get; set; }


        [JsonPropertyName("data")]
        public Data Data { get; set; }

        [JsonPropertyName("poolId")]
        public int PoolId { get; set; }

        [JsonPropertyName("agentSpecification")]
        public AgentSpecification AgentSpecification { get; set; }

        [JsonPropertyName("orchestrationId")]
        public string OrchestrationId { get; set; }

        [JsonPropertyName("matchesAllAgentsInPool")]
        public bool MatchesAllAgentsInPool { get; set; }

        [JsonPropertyName("priority")]
        public int Priority { get; set; }

        public override string ToString()
        {
            return $"{Result}=> Request: {RequestId}; Queued: {QueueTime}; Assgined: {AssignTime}; Pipeline: {Definition.Name}; Agent: {this.AgentSpecification.VMImage}";
        }
    }

    public class JobRequestCollection
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("value")]
        public List<JobRequest> Value { get; set; }
    }


    #endregion

    #region Undocumented Jobs
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
 
    public class MsVssBuildWebAgentJobsData
    {

        [JsonPropertyName("ms.vss-build-web.agent-jobs-data-provider")]
        public MsVssBuildWebAgentJobsDataProvider MsVssBuildWebAgentJobsDataProvider { get; set; }
    }

 

    public class Job
    {
        [JsonPropertyName("requestId")]
        public int RequestId { get; set; }

        [JsonPropertyName("queueTime")]
        public DateTimeOffset QueueTime { get; set; }

        [JsonPropertyName("assignTime")]
        public DateTimeOffset AssignTime { get; set; }

        [JsonPropertyName("receiveTime")]
        public DateTimeOffset ReceiveTime { get; set; }

        [JsonPropertyName("finishTime")]
        public DateTimeOffset FinishTime { get; set; }

        [JsonPropertyName("result")]
        public int Result { get; set; }

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

        [JsonPropertyName("definition")]
        public Definition Definition { get; set; }


        [JsonPropertyName("data")]
        public Data Data { get; set; }

        [JsonPropertyName("poolId")]
        public int PoolId { get; set; }

        [JsonPropertyName("orchestrationId")]
        public string OrchestrationId { get; set; }

        [JsonPropertyName("matchesAllAgentsInPool")]
        public bool MatchesAllAgentsInPool { get; set; }

        [JsonPropertyName("priority")]
        public int Priority { get; set; }

        public bool IsCompleted
        {
            get 
            {                
                return this.FinishTime.UtcDateTime > new DateTime(2000, 1, 1).ToUniversalTime();
            }
        }

        public override string ToString()
        {
            return $"Completed:{this.IsCompleted}; QT:{QueueTime}; AT:{this.AssignTime}; RT:{this.ReceiveTime};FT:{this.FinishTime}";
        }
    }

    public enum JobStatus
    {
        Succeeded,
        Queueed,
        Failed,
        Cancelled
    }

    public class MsVssBuildWebAgentJobsDataProvider
    {
        [JsonPropertyName("jobs")]
        public List<Job> Jobs { get; set; }
    }


    public class DataProviders
    {
        [JsonPropertyName("data")]
        public MsVssBuildWebAgentJobsData Data { get; set; }
    }

    public class Fps
    {
        [JsonPropertyName("dataProviders")]
        public DataProviders DataProviders { get; set; }
    }

    public class UndocumentedJobs
    {
        [JsonPropertyName("fps")]
        public Fps Fps { get; set; }
    }


    #endregion
}
