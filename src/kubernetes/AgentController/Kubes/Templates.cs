

using k8s;
using k8s.Models;
using System.Threading.Tasks;

namespace AgentController.Kubes
{
    public class Templates
    {
        private const string POD_TEMPLATE = "PodTemplate.k8s";

        public static async Task<V1Pod> ReadPodTemplateAsync()
        {
            using var stream = typeof(Templates).Assembly.GetManifestResourceStream($"{typeof(Templates).Namespace}.{POD_TEMPLATE}");
            var pod = await Yaml.LoadFromStreamAsync<V1Pod>(stream);
            return pod;
        }
    }
}
