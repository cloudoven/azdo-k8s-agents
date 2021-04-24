

using AgentController.AzDO.Supports;
using AgentController.Supports;
using k8s;
using k8s.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace AgentController.Kubes
{
    public class K8sUtils
    {
        private readonly Kubernetes client;
        public K8sUtils(Kubernetes client)
        {
            this.client = client;
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

            await client.CreateNamespacedPodAsync(podSpec, ns);
            return true;
        }

        public async Task<CRDList> ListAgentSpecAsync(string ns)
        {
            try
            {
                var response = await client.ListNamespacedCustomObjectAsync(CrdConstants.Group,
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
            return default(CRDList);
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
    }
}
