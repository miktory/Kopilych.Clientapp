using Kopilych.Application.Interfaces;
using Kopilych.Application.Interfaces.Repository;
using Kopilych.ExternalCommunication.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.ExternalCommunication
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddExternalCommunication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IApiCommunicatorService, ApiCommunicatorService>();
            services.AddHttpClient();
            return services;
        }
    }
}
