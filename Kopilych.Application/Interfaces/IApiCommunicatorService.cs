using Kopilych.Shared.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.Interfaces
{
    public interface IApiCommunicatorService 
    {
        string AccessToken { get;  }
        void SetAccessToken(string token);
        // Метод для выполнения GET-запроса
        Task<TResponse> GetAsync<TResponse>(string endpoint, CancellationToken ctoken = default);

        // Метод для выполнения POST-запроса
        Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest data, CancellationToken ctoken = default);

        // Метод для выполнения PATCH-запроса
        Task<TResponse> PatchAsync<TRequest, TResponse>(string endpoint, TRequest data, CancellationToken ctoken = default);
        // Метод для выполнения PUT-запроса
        Task<TResponse> PutAsync<TRequest, TResponse>(string endpoint, TRequest data, CancellationToken ctoken = default);

        // Метод для выполнения DELETE-запроса
        Task DeleteAsync(string endpoint, CancellationToken ctoken = default);
    }
}
