using Kopilych.Application.Interfaces;
using Kopilych.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls;
using System.Reflection;
using static Kopilych.Application.DependencyInjection;
using static System.Formats.Asn1.AsnWriter;

namespace Kopilych.Mobile
{
    public static class MauiProgram
    {

        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("Fonts/Nunito/Nunito-SemiBold.ttf", "NunitoSemibold");
                    fonts.AddFont("Fonts/Nunito/Nunito-ExtraBold.ttf", "NunitoExtrabold");
                    fonts.AddFont("Fonts/Nunito/Nunito-Bold.ttf", "NunitoBold");
                });

            var a = Assembly.GetExecutingAssembly();

            var stream = FileSystem.OpenAppPackageFileAsync("appsettings.json").Result;
            var configuration = new ConfigurationBuilder().AddJsonStream(stream).Build(); 
#if DEBUG
            builder.Logging.AddDebug();
#endif
            builder.Configuration.AddConfiguration(configuration);
            builder.Services.AddSingleton<IConfiguration>(configuration);

            var services = builder.Services;
            services.AddApplication(configuration);
            services.AddPersistence(configuration);
            var app = builder.Build();



            using (var scope = app.Services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                    var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
                    DbInitializer.Initialize(context);
  
            }

            return app;
        }
    }
}
