

using AgentController.AzDO.Supports;
using AgentController.Supports;
using k8s;
using k8s.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AgentController.Kubes
{
    public class K8sUtils
    {
        private readonly Kubernetes _client;
        private Timer _timer;
        private Watcher<V1Pod> _podWatcher;
        private const int _intervalInSeconds = 1000 * 45; // 45 seconds
        public K8sUtils(Kubernetes client)
        {
            _client = client;
            _timer = null;
        }

        public async Task<bool> SpinAgentAsync(ConfigUtils.Config cfg)
        {
            return await SpinAgentAsync(cfg.targetNamespace, cfg.orgUri, cfg.pat, cfg.poolName);
        }

        private async Task<bool> SpinAgentAsync(string ns, string azdoUri, string pat, string poolName)
        {
            var agentSpec = await GetAgentSpecAsync(ns);
            var podSpec = await Templates.ReadPodTemplateAsync();
            var name = agentSpec.GeneratePodName();

            podSpec.Metadata.NamespaceProperty = ns;
            podSpec.Metadata.Name = name;
            var containerSpec = podSpec.Spec.Containers.FirstOrDefault();
            containerSpec.Name = name;
            containerSpec.Image = agentSpec.GetImageName();

            if (containerSpec.Env == null)
            {
                containerSpec.Env = new List<V1EnvVar>();
            }
            containerSpec.Env.Add(new V1EnvVar("AZP_POOL", poolName));
            containerSpec.Env.Add(new V1EnvVar("AZP_URL", azdoUri));
            containerSpec.Env.Add(new V1EnvVar("AZP_TOKEN", pat));

            await _client.CreateNamespacedPodAsync(podSpec, ns);
            return true;
        }

        public async Task<CRDList> ListAgentSpecAsync(string ns)
        {
            try
            {
                var response = await _client.ListNamespacedCustomObjectAsync(CrdConstants.Group,
                                                   CrdConstants.Version,
                                                   ns,
                                                   CrdConstants.CRD_AgentSpecPlural);
                var crds = JsonSerializer.Deserialize<CRDList>(response.ToString(), HttpClientExtensions.CamelCaseOption);
                return crds;
            }
            catch 
            {
                // swallow for now
            }
            return default;
        }
        
        public async Task<AgentSpec> GetAgentSpecAsync(string ns)
        {
            var crdCollection = await ListAgentSpecAsync(ns);
            if(crdCollection != null && crdCollection.Items != null && crdCollection.Items.Any())
            {
                return crdCollection.Items.First().Spec;
            }
            return default(AgentSpec);
        }

        public void WatchAsync(
            string targetNamespace, 
            Func<WatchEventType, V1Pod, Task> callback)
        {
            _timer = new Timer(
                _ => 
                {
                    Console.WriteLine($"Reestablishing the connection to the event stream.");
                    if (_podWatcher != null)
                    {
                        Console.WriteLine($"Disposing last watcher instance.");
                        _podWatcher.Dispose();
                    }
                    var podlistResp = _client.ListNamespacedPodWithHttpMessagesAsync(targetNamespace, watch: true);
                    _podWatcher = podlistResp.Watch<V1Pod, V1PodList>(async (eventType, pod) =>
                    {
                        await callback(eventType, pod);
                    });
                },
                state: null,
                dueTime: 0,
                period: _intervalInSeconds);
        }
    }
}
