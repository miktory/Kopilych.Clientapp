using Kopilych.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Kopilych.ExternalCommunication.Services
{
    internal class ApiCommunicatorService : IApiCommunicatorService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        public ApiCommunicatorService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
        }




        public string AccessToken { get; private set; }

        public async Task<TResponse> GetAsync<TResponse>(string endpoint, CancellationToken ctoken)
        {
            var httpClient = _httpClientFactory.CreateClient();
            // Установка заголовков аутентификации, если токен доступен
            if (!string.IsNullOrEmpty(AccessToken))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
            }

            var response = await httpClient.GetAsync(endpoint, ctoken);
            response.EnsureSuccessStatusCode();


            TResponse result;
            var stringResponse = await response.Content.ReadAsStringAsync(ctoken);
            try
            {
                result = JsonSerializer.Deserialize<TResponse>(stringResponse, _jsonSerializerOptions);
            }
            catch (JsonException ex)
            {
                if (typeof(TResponse) == typeof(byte[]))
                {
                    var bytes = await response.Content.ReadAsByteArrayAsync(ctoken);
                    return (TResponse)(object)bytes;
                }
                result = (TResponse)(object)stringResponse;
            }
            return result;
        }

        public async Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest data, CancellationToken ctoken = default)
        {
            var httpClient = _httpClientFactory.CreateClient();
            if (!string.IsNullOrEmpty(AccessToken))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
            }

            var jsonContent = JsonSerializer.Serialize(data, _jsonSerializerOptions);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(endpoint, httpContent, ctoken);
            response.EnsureSuccessStatusCode();


            TResponse result;
            var stringResponse = await response.Content.ReadAsStringAsync(ctoken);
            try
            {
                result = JsonSerializer.Deserialize<TResponse>(stringResponse, _jsonSerializerOptions);
            }
            catch (JsonException ex)
            {
                if (typeof(TResponse) == typeof(byte[]))
                {
                    var bytes = await response.Content.ReadAsByteArrayAsync(ctoken);
                    return (TResponse)(object)bytes;
                }

                result = (TResponse)(object)stringResponse;
            }
            return result;
        }

        public async Task<TResponse> PatchAsync<TRequest, TResponse>(string endpoint, TRequest data, CancellationToken ctoken = default)
        {
            var httpClient = _httpClientFactory.CreateClient();
            if (!string.IsNullOrEmpty(AccessToken))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
            }

            var jsonContent = JsonSerializer.Serialize(data, _jsonSerializerOptions);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await httpClient.PatchAsync(endpoint, httpContent, ctoken);
            response.EnsureSuccessStatusCode();

          
            TResponse result;
            var stringResponse = await response.Content.ReadAsStringAsync(ctoken);
            try
            {
                result = JsonSerializer.Deserialize<TResponse>(stringResponse, _jsonSerializerOptions);
            }
            catch (JsonException ex) 
            {
                if (typeof(TResponse) == typeof(byte[]))
                {
                    var bytes = await response.Content.ReadAsByteArrayAsync(ctoken);
                    return (TResponse)(object)bytes;
                }
                result = (TResponse)(object)stringResponse;
            }
            return result;
        }

        public async Task<TResponse> PutAsync<TRequest, TResponse>(string endpoint, TRequest data, CancellationToken ctoken = default)
        {
            var httpClient = _httpClientFactory.CreateClient();
            if (!string.IsNullOrEmpty(AccessToken))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
            }

            var jsonContent = JsonSerializer.Serialize(data, _jsonSerializerOptions);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await httpClient.PutAsync(endpoint, httpContent, ctoken);
            response.EnsureSuccessStatusCode();


            TResponse result;
            var stringResponse = await response.Content.ReadAsStringAsync(ctoken);
            try
            {
                result = JsonSerializer.Deserialize<TResponse>(stringResponse, _jsonSerializerOptions);
            }
            catch (JsonException ex)
            {
                if (typeof(TResponse) == typeof(byte[]))
                {
                    var bytes = await response.Content.ReadAsByteArrayAsync(ctoken);
                    return (TResponse)(object)bytes;
                }
                result = (TResponse)(object)stringResponse;
            }
            return result;
        }



        public async Task DeleteAsync(string endpoint, CancellationToken ctoken = default)
        {
            var httpClient = _httpClientFactory.CreateClient();
            if (!string.IsNullOrEmpty(AccessToken))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
            }
            var response = await httpClient.DeleteAsync(endpoint, ctoken);
            response.EnsureSuccessStatusCode();
        }

        public void SetAccessToken(string token)
        {
            AccessToken = token;
            // Вы можете добавить дополнительную логику, если необходимо
        }

    }
}
