using FluentValidation;
using Kopilych.Application.Common.Behaviors;
using Kopilych.Application.Common.Mappings;
using Kopilych.Application.Interfaces;
using Kopilych.Application.Services;
using Kopilych.WebApi;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration cfg)
		{
			services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
			services.AddAutoMapper(config =>
			{
				config.AddProfile(new MappingConfiguration(config));
			});

            services.Configure<ApiEndpoints>(cfg.GetSection("ApiEndpoints"));
            services.AddSingleton<ApiEndpoints>(s => s.GetRequiredService<IOptions<ApiEndpoints>>().Value);
            services.AddSingleton<IIntegrationService, IntegrationService>();

            services.AddScoped<ISetupWizardService, SetupWizardService>();
            services.AddValidatorsFromAssemblies(new[] { Assembly.GetExecutingAssembly() });
			services.AddTransient(typeof(IPipelineBehavior<,>),
				typeof(ValidationBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>),
    typeof(ValidationBehaviorAdditional<,>));
            services.Configure<UserRestrictionsSettings>(cfg.GetSection("UserRestrictions"));
            services.AddSingleton<IUserRestrictionsSettings>(s => s.GetRequiredService<IOptions<UserRestrictionsSettings>>().Value);
            services.AddTransient(typeof(IUserInfoService),
            typeof(UserInfoService));
            services.AddTransient(typeof(IPiggyBankService),
          typeof(PiggyBankService));
            services.AddTransient(typeof(ITransactionService),
      typeof(TransactionService));



            return services;
		}
	}
}
