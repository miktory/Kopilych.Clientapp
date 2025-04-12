using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Kopilych.Persistence
{
	public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
	{
		public ApplicationDbContext CreateDbContext(string[] args)
		{
			// При использовании EF TOOLS необходимо передавать connectionString в качестве аргумента.
			// Например, вот так: Remove-Migration -StartupProject "Kopilych.Persistence" -Args "..."
			var connectionString = "";
			if (args != null && args.Length > 0)
			{
				// Предполагается, что первый аргумент - это строка соединения
				connectionString = args[0];
			}
			else
			{
				//var path = Path.GetDirectoryName(Assembly.Load("Kopilych.WebApi").Location);
				//var configuration = new ConfigurationBuilder()
				//	.SetBasePath(path)
				//	.AddJsonFile("appsettings.json")
				//	.Build();


				//connectionString = configuration["DbConnection"];
			}
			var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
			builder.UseSqlite(connectionString);

			return new ApplicationDbContext(builder.Options);
		}
	}
}
