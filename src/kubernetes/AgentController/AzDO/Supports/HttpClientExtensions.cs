using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AgentController.AzDO.Supports
{
    public static class HttpClientExtensions
    {
        public static JsonSerializerOptions CamelCaseOption = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        public static async Task<string> GetRestJsonAsync(
            this HttpClient client, string requestPath)
        {   
            var response = await client.GetAsync(requestPath);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            return default;
        }


        public static async Task<byte[]> GetImageRestAsync(
            this HttpClient client, string requestPath)
        {
            var response = await client.GetAsync(requestPath);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsByteArrayAsync();
            }
            return default(byte[]);
        }

        public static async Task<TPayload> GetRestAsync<TPayload>(
            this HttpClient client, string requestPath, JsonSerializerOptions options = null)
        {
            var response = await client.GetAsync(requestPath);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadContentAsync<TPayload>(options);
            }
            return default;
        }

        public static async Task<bool> DeleteRestAsync(
            this HttpClient client, string requestPath)
        {   
            var response = await client.DeleteAsync(requestPath);
            return response.IsSuccessStatusCode;
        }

        public static async Task<string> PutRestAsync(
          this HttpClient client, string requestPath, 
          string payload)
        {
            var jsonContent = new StringContent(payload, Encoding.UTF8, "application/json");
            var response = await client.PutAsync(requestPath, jsonContent);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            return string.Empty;
        }

        public static async Task<TResponsePayload> PutRestAsync<TRequestPayload, TResponsePayload>(
           this HttpClient client, string requestPath, TRequestPayload payload)
        {
            var jsonString = JsonSerializer.Serialize(payload, CamelCaseOption);
            var jsonContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = await client.PutAsync(requestPath, jsonContent);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadContentAsync<TResponsePayload>();
            }
            return default;
        }

        public static async Task<HttpResponseMessage> PatchAsync(
            HttpClient client, string requestUri, StringContent iContent)
        {
            var method = new HttpMethod("PATCH");
            var request = new HttpRequestMessage(method, requestUri)
            {
                Content = iContent
            };            
            var response = await client.SendAsync(request);
            return response;
        }

        public static async Task<string> PatchRestAsync(
         this HttpClient client, string requestPath, string payload)
        {
            var jsonContent = new StringContent(payload, Encoding.UTF8, "application/json");
            var response = await PatchAsync(client, requestPath, jsonContent);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            return string.Empty;
        }

        public static async Task<string> PostRestAsync(
         this HttpClient client, string requestPath, object payload)
        {
            var jsonString = string.Empty;
            if (payload is string)
            {
                jsonString = payload.ToString();
            }
            else
            {
                jsonString = JsonSerializer.Serialize(payload, CamelCaseOption);
            }
            var jsonContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(requestPath, jsonContent);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            return string.Empty;
        }

        public static async Task<TResponseType> PostRestAsync<TResponseType>(
         this HttpClient client, string requestPath, object payload)
        {
            var jsonString = JsonSerializer.Serialize(payload, CamelCaseOption);
            var jsonContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(requestPath, jsonContent);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadContentAsync<TResponseType>();
            }
            return default;
        }

        public static async Task<TPayload> ReadContentAsync<TPayload>(this HttpContent content, 
            JsonSerializerOptions options = null)
        {
            if (options == null) { options = CamelCaseOption; }
            var contentString = await content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TPayload>(contentString, options);
        }
    }
}
