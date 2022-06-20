
using System;
using AgentController.AzDO;
using System.Linq;
using System.Threading.Tasks;
using k8s;
using AgentController.Kubes;
using k8s.Models;
using System.Collections.Concurrent;
using AgentController.Supports;


var cfg = ConfigUtils.Get();

var config = cfg.clusterMode ?
    KubernetesClientConfiguration.InClusterConfig()
    : KubernetesClientConfiguration.BuildConfigFromConfigFile();
var client = new Kubernetes(config);
var k8sUtil = new K8sUtils(client);
var agentService = new AgentService(cfg.orgUri, cfg.pat);
var pool = await agentService.GetPoolByNameAsync(cfg.poolName);
if (pool != null)
{
    var eventSideEffectsMitigations = new ConcurrentDictionary<string, string>();
    var danglingAgents = new ConcurrentQueue<V1Pod>();

    k8sUtil.WatchAsync(cfg.targetNamespace, async (eventType, pod) =>
    {
        if (pod.IsInSuccededOrFailedPhase(eventType))
        {
            // keep them in garbage bin for now
            danglingAgents.Enqueue(pod);
            await Task.CompletedTask;
        }
    });

    //var podlistResp = client.ListNamespacedPodWithHttpMessagesAsync(cfg.targetNamespace, watch: true);
    //using (podlistResp.Watch<V1Pod, V1PodList>(async (eventType, pod) =>
    //{
    //    if (pod.IsInSuccededOrFailedPhase(eventType))
    //    {
    //        // keep them in garbage bin for now
    //        danglingAgents.Enqueue(pod);
    //        await Task.CompletedTask;
    //    }
    //}))

    while (true)
    {
        var jobs = await agentService.ListJobRequestsUIAsync(pool.Id);
        var activeJob = (jobs.Count() - jobs.Count(j => j.IsCompleted));
        var podCollection = await client.ListNamespacedPodAsync(cfg.targetNamespace);
        // get only pending and running pods
        var totalNumberOfPods = podCollection.Items.Where(pod => pod.IsActive()).Count();

        while (totalNumberOfPods < cfg.maxAgentsCount && totalNumberOfPods < (activeJob + cfg.standBy))
        {
            Console.WriteLine($"Responding to demand (active-request={activeJob}; current-pod-count={totalNumberOfPods}) Launching new Pod");
            await k8sUtil.SpinAgentAsync(cfg);
            ++totalNumberOfPods;
        }

        while (!danglingAgents.IsEmpty)
        {
            if (danglingAgents.TryDequeue(out V1Pod pod) && pod != null)
            {
                if (eventSideEffectsMitigations.TryAdd(pod.Metadata.Name, pod.Metadata.NamespaceProperty))
                {
                    Console.WriteLine($"Deleting Pod={pod.Metadata.Name} (NS={pod.Metadata.NamespaceProperty}).");
                    await client.DeleteNamespacedPodAsync(pod.Metadata.Name, pod.Metadata.NamespaceProperty);
                    Console.WriteLine($"Deleted Pod={pod.Metadata.Name} (NS={pod.Metadata.NamespaceProperty}).");

                    var agent = await agentService.GetAgentByNameAsync(pool.Id, pod.Metadata.Name);
                    if (agent != null)
                    {
                        Console.WriteLine($"Deleting Agent={pod.Metadata.Name}(ID={agent.Id}) (POOL={pool.Id}).");
                        await agentService.DeleteAgentAsync(pool.Id, agent.Id);
                        Console.WriteLine($"Deleted Agent={pod.Metadata.Name}(ID={agent.Id}) (POOL={pool.Id}).");
                    }
                }
                else
                {
                    eventSideEffectsMitigations.TryRemove(pod.Metadata.Name, out var discardedValue);
                    Console.WriteLine($"Skipping side-effect of DELETE events Pod={pod.Metadata.Name} (NS={pod.Metadata.NamespaceProperty})");
                }
            }
        }

        await Task.Delay(1000);
    }

}
