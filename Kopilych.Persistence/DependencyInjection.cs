using Kopilych.Application.Interfaces.Repository;
using Kopilych.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Persistence
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
		{
			var dbfilename = configuration["DbFilename"];
			//services.AddDbContext<ApplicationDbContext>(options =>
			//{
			//	options.UseNpgsql(connectionString);
			//});
			services.AddDbContextFactory<ApplicationDbContext>(options =>
			{
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), dbfilename);
                options.UseSqlite($"Data Source={path}");
                //options.EnableSensitiveDataLogging();
            });

			services.AddScoped<ApplicationDbContext>(p => p.GetRequiredService<IDbContextFactory<ApplicationDbContext>>().CreateDbContext());

			services.AddScoped<IPiggyBankRepository, PiggyBankRepository>();
			services.AddScoped<IUserRepository, UserRepository>();
			services.AddScoped<IUserFriendshipRepository, UserFriendshipRepository>();
			services.AddScoped<IUserPiggyBankRepository, UserPiggyBankRepository>();
            services.AddScoped<IPiggyBankCustomizationRepository, PiggyBankCustomizationRepository>();
            services.AddScoped<IPremiumUserRepository, PremiumUserRepository>();
            services.AddScoped<IPaymentTypeRepository, PaymentTypeRepository>();
            services.AddScoped<IPiggyBankTypeRepository, PiggyBankTypeRepository>();
            services.AddScoped<ITransactionTypeRepository, TransactionTypeRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<IUserSessionRepository, UserSessionRepository>();

            return services;
		}
	}
}
