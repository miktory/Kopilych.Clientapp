using Kopilych.Application.Common.Exceptions;
using Kopilych.Application.Interfaces.Repository;
using Kopilych.Shared.View_Models;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.CQRS.Queries.Auth.Token.GetTokenByAuthCode
{
	public class GetTokenByAuthCodeQueryHandler : IRequestHandler<GetTokenByAuthCodeQuery, HttpResponseMessage>
	{
		private readonly HttpClient _httpClient;
		public GetTokenByAuthCodeQueryHandler(HttpClient httpClient) => _httpClient = httpClient;
		public async Task<HttpResponseMessage> Handle(GetTokenByAuthCodeQuery request, CancellationToken cancellationToken)
		{
			var url = request.Endpoint;


			// Форматирование данных запроса
			var content = new FormUrlEncodedContent(new[]
			{
			new KeyValuePair<string, string>("grant_type", "authorization_code"),
			new KeyValuePair<string, string>("code", request.AuthCode.ToString()),
			new KeyValuePair<string, string>("client_id", request.ClientId),
			new KeyValuePair<string, string>("client_secret", request.ClientSecret),
			new KeyValuePair<string, string>("redirect_uri", request.RedirectUri)
        });

			// Отправка POST запроса
			var response = await _httpClient.PostAsync(url, content, cancellationToken);

			return response;
		}

	}
}
