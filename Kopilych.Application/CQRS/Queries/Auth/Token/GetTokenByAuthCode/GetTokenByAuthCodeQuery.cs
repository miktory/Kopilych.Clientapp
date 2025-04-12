using Kopilych.Shared.View_Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.CQRS.Queries.Auth.Token.GetTokenByAuthCode
{
    public class GetTokenByAuthCodeQuery : IRequest<HttpResponseMessage>
    {
        public string Endpoint { get; set; }
        public string AuthCode {  get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string RedirectUri { get; set; }
    }
}
