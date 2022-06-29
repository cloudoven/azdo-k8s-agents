using AgentController.AzDO.Supports;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AgentController.AzDO
{
    public abstract class RestServiceBase
    {
        private readonly string pat;
        private readonly string adoUrl;
        private static readonly HttpClient coreApiClient = new();
        public RestServiceBase(string uri, string pat)
        {
            if (string.IsNullOrWhiteSpace(uri)) throw new ArgumentNullException(nameof(uri));
            if (string.IsNullOrWhiteSpace(pat)) throw new ArgumentNullException(nameof(pat));

            if (uri.EndsWith("/"))
            {
                uri = uri.TrimEnd('/');
            }
            this.adoUrl = uri;
            this.pat = pat;

            coreApiClient.BaseAddress = new Uri(adoUrl);
            var credentials =
                     Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(
                         string.Format("{0}:{1}", "", this.pat)));
            coreApiClient.DefaultRequestHeaders.Accept.Clear();
            coreApiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            coreApiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
        }

        protected HttpClient Http { get { return coreApiClient; } }
        protected string OrgName
        {
            get 
            {
                return new Uri(this.adoUrl).AbsolutePath.Replace("/", string.Empty);
            }            
        }

        protected JsonSerializerOptions Options
        {
            get
            {
                var option = new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                option.Converters.Add(new UnixEpochDateConverter());
                option.Converters.Add(new JsonStringEnumConverter());

                return option;
            }
        }
    }
}
