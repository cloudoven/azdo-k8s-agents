﻿
using System;
using AgentController.AzDO;
using System.Linq;
using System.Threading.Tasks;
using k8s;
using AgentController.Kubes;
using k8s.Models;
using System.Collections.Concurrent;
using AgentController.Supports;
using System.Collections.Generic;

var cfg = ConfigUtils.Get();
var instrumentation = new InstrumentationClient(cfg.AppInsightConnectionString, cfg.DisableConsoleLogs);
var config = cfg.ClusterMode ?
    KubernetesClientConfiguration.InClusterConfig()
    : KubernetesClientConfiguration.BuildConfigFromConfigFile();
var client = new Kubernetes(config);
var k8sUtil = new K8sUtils(client, instrumentation);
var agentService = new AgentService(cfg.OrgUri, cfg.Pat);
var pool = await agentService.GetPoolByNameAsync(cfg.PoolName);

if (pool != null)
{
    var eventSideEffectsMitigations = new ConcurrentDictionary<string, string>();
    var danglingAgents = new ConcurrentQueue<V1Pod>();

    k8sUtil.WatchAsync(cfg.TargetNamespace, async (eventType, pod) =>
    {
        if (pod.IsInSuccededOrFailedPhase(eventType))
        {
            // keep them in garbage bin for now
            danglingAgents.Enqueue(pod);
            await Task.CompletedTask;
        }
    });

    while (true)
    {
        var jobs = await agentService.ListJobRequestsUIAsync(pool.Id);
        var activeJob = (jobs.Count() - jobs.Count(j => j.IsCompleted));
        var podCollection = await client.ListNamespacedPodAsync(cfg.TargetNamespace);
        // get only pending and running pods
        var totalNumberOfPods = podCollection.Items.Where(pod => pod.IsActive()).Count();

        while (totalNumberOfPods < cfg.MaxAgentsCount && totalNumberOfPods < (activeJob + cfg.StandBy))
        {
            instrumentation
                .TrackEvent($"Responding to demand (active-request={activeJob}; current-pod-count={totalNumberOfPods}) Launching new Pod",
                new Dictionary<string, string> 
                {
                    { "TargetNamespace", cfg.TargetNamespace },
                    { "ActiveJobRequest", activeJob.ToString() },
                    { "PodCount", totalNumberOfPods.ToString() }
                });
            await k8sUtil.SpinAgentAsync(cfg);
            ++totalNumberOfPods;
        }

        while (!danglingAgents.IsEmpty)
        {
            if (danglingAgents.TryDequeue(out V1Pod pod) && pod != null)
            {
                if (eventSideEffectsMitigations.TryAdd(pod.Metadata.Name, pod.Metadata.NamespaceProperty))
                {
                    instrumentation
                        .TrackEvent($"Deleting Pod={pod.Metadata.Name}",
                        new Dictionary<string, string>
                        {
                            { "PodNamespace", pod.Metadata.NamespaceProperty },
                            { "PodName", pod.Metadata.Name }
                        });                    
                    await client.DeleteNamespacedPodAsync(pod.Metadata.Name, pod.Metadata.NamespaceProperty);
                    instrumentation
                        .TrackEvent($"Deleted Pod={pod.Metadata.Name}",
                        new Dictionary<string, string>
                        {
                            { "PodNamespace", pod.Metadata.NamespaceProperty },
                            { "PodName", pod.Metadata.Name }
                        });

                    var agent = await agentService.GetAgentByNameAsync(pool.Id, pod.Metadata.Name);
                    if (agent != null)
                    {
                        instrumentation
                            .TrackEvent($"Deleting Agent={pod.Metadata.Name}(ID={agent.Id}) (POOL={pool.Id}).",
                            new Dictionary<string, string>
                            {
                                { "AgentName", pod.Metadata.Name },
                                { "AgentID", agent.Id.ToString() },
                                { "PoolID", pool.Id.ToString() },
                                { "PoolName", pool.Name.ToString() }
                            });
                        await agentService.DeleteAgentAsync(pool.Id, agent.Id);
                        instrumentation
                            .TrackEvent($"Deleted Agent={pod.Metadata.Name}(ID={agent.Id}) (POOL={pool.Id}).",
                            new Dictionary<string, string>
                            {
                                { "AgentName", pod.Metadata.Name },
                                { "AgentID", agent.Id.ToString() },
                                { "PoolID", pool.Id.ToString() },
                                { "PoolName", pool.Name.ToString() }
                            });
                    }
                }
                else
                {
                    eventSideEffectsMitigations.TryRemove(pod.Metadata.Name, out var discardedValue);
                    

                    instrumentation
                        .TrackEvent($"Skipping side-effect of DELETE events Pod={pod.Metadata.Name}",
                        new Dictionary<string, string>
                        {
                                { "PodName", pod.Metadata.Name },
                                { "PodNamespace", pod.Metadata.NamespaceProperty }
                        });
                }
            }
        }
        await Task.Delay(1000);
    }

}
