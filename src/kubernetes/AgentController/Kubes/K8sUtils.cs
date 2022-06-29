

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
        private readonly InstrumentationClient _instrumentationClient;
#pragma warning disable IDE0052 // Remove unread private members
        private Timer _timer;
#pragma warning restore IDE0052 // Remove unread private members
        private Watcher<V1Pod> _podWatcher;
        private const int _intervalInSeconds = 1000 * 45; // 45 seconds
        public K8sUtils(Kubernetes client, InstrumentationClient instrumentationClient)
        {
            _client = client;
            _instrumentationClient = instrumentationClient;
            _timer = null;
        }

        public async Task<bool> SpinAgentAsync(ConfigUtils.Config cfg, Dictionary<string, string> labels)
        {
            return await SpinAgentAsync(cfg.TargetNamespace, cfg.OrgUri, cfg.Pat, cfg.PoolName, labels);
        }

        private async Task<bool> SpinAgentAsync(
            string ns, 
            string azdoUri, 
            string pat, 
            string poolName,
            Dictionary<string, string> labels)
        {
            try
            {
                var agentSpec = await GetAgentSpecAsync(ns);
                var podSpec = await Templates.ReadPodTemplateAsync();
                var name = agentSpec.GeneratePodName();

                podSpec.Metadata.NamespaceProperty = ns;
                podSpec.Metadata.Name = name;

                if(labels != null)
                {   
                    foreach (var kv in labels)
                    {
                        podSpec.SetLabel(kv.Key, kv.Value);
                    }
                }
                
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

                _instrumentationClient.TrackEvent("Pod Created", new Dictionary<string, string>
                {
                    { "PodNamespace", ns },
                    { "Organization", azdoUri },
                    { "PoolName", poolName },
                    { "ImageName", containerSpec.Image },
                    { "PodName", podSpec.Metadata.Name },
                });

                return true;
            }
            catch(Exception ex)
            {
                _instrumentationClient.TrackError(ex);
            }
            return false;
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
            catch (Exception ex) 
            {
                _instrumentationClient.TrackError(ex);
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
            return default;
        }

        public void WatchAsync(
            string targetNamespace, 
            Func<WatchEventType, V1Pod, Task> callback)
        {
            _timer = new Timer(
                _ => 
                {
                    _instrumentationClient.TrackEvent("Reestablishing watch connection to the Kubernetes API server.", new Dictionary<string, string>
                    {
                        { "PodNamespace", targetNamespace },
                        { "Watch interval", _intervalInSeconds.ToString() }
                    }, false);
                    if (_podWatcher != null)
                    {   
                        _instrumentationClient.TrackEvent("Disposing last used watcher instance.", new Dictionary<string, string>
                        {
                            { "PodNamespace", targetNamespace }
                        }, false);
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
