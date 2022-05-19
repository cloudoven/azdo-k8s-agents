using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AgentController.Kubes
{
    public class CRDMetadata
    {
        [JsonPropertyName("creationTimestamp")]
        public DateTime CreationTimestamp { get; set; }

        [JsonPropertyName("generation")]
        public int Generation { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("namespace")]
        public string Namespace { get; set; }

        [JsonPropertyName("resourceVersion")]
        public string ResourceVersion { get; set; }

        [JsonPropertyName("selfLink")]
        public string SelfLink { get; set; }

        [JsonPropertyName("uid")]
        public string Uid { get; set; }

        [JsonPropertyName("continue")]
        public string Continue { get; set; }
    }

    public class AgentSpec
    {
        [JsonPropertyName("image")]
        public string Image { get; set; }

        [JsonPropertyName("imagePullSecretName")]
        public string ImagePullSecretName { get; set; }

        [JsonPropertyName("prefix")]
        public string Prefix { get; set; }

        public string GeneratePodName()
        {
            return $"{(string.IsNullOrWhiteSpace(Prefix) ? "agent" : Prefix)}-{DateTime.UtcNow.Ticks}";
        }

        public string GetImageName()
        {
            return string.IsNullOrWhiteSpace(Image) ? "moimhossain/azdo-agent-linux-x64:latest" : Image;
        }
    }

    public class CRD
    {
        [JsonPropertyName("apiVersion")]
        public string ApiVersion { get; set; }

        [JsonPropertyName("kind")]
        public string Kind { get; set; }

        [JsonPropertyName("metadata")]
        public CRDMetadata Metadata { get; set; }

        [JsonPropertyName("spec")]
        public AgentSpec Spec { get; set; }
    }

    public class CRDList
    {
        [JsonPropertyName("apiVersion")]
        public string ApiVersion { get; set; }

        [JsonPropertyName("items")]
        public List<CRD> Items { get; set; }

        [JsonPropertyName("kind")]
        public string Kind { get; set; }

        [JsonPropertyName("metadata")]
        public CRDMetadata Metadata { get; set; }
    }
}
