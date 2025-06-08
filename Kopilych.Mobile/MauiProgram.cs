using Android.Content;
using AutoMapper;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Core;
using Java.Security;
using Kopilych.Application.Interfaces;
using Kopilych.ExternalCommunication;
using Kopilych.Mobile.Interfaces;
using Kopilych.Mobile.Middleware;
using Kopilych.Mobile.Services;
using Kopilych.Mobile.View_Models;
using Kopilych.Mobile.Views;
using Kopilych.Persistence;
using Kopilych.Shared;
using Kopilych.Shared.DTO;
using Kopilych.Shared.View_Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.CompilerServices;
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
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("Nunito-SemiBold.ttf", "NunitoSemibold");
                    fonts.AddFont("Nunito-ExtraBold.ttf", "NunitoExtrabold");
                    fonts.AddFont("Nunito-Bold.ttf", "NunitoBold");
                    fonts.AddFont("Nunito-Regular.ttf", "NunitoRegular");
                });
            var a = Assembly.GetExecutingAssembly();

            var stream = FileSystem.OpenAppPackageFileAsync("appsettings.json").Result;
            var configuration = new ConfigurationBuilder().AddJsonStream(stream).Build(); 
#if DEBUG
            builder.Logging.AddDebug();
#endif

            builder.Configuration.AddConfiguration(configuration);

            builder.Services.AddSingleton<INavigationService>(provider =>
            {
                // Получить конфигурацию навигации, например, главный NavigationPage
                var navigation = Shell.Current.Navigation;/* получить экземпляр NavigationPage, например, из MainPage */
                var svc = new NavigationService(navigation);
                Shell.Current.Navigated += (sender, args) =>
                {
                    // Вызываем событие, уведомляя об изменении стека навигации
                    svc.OnNavigationStackChanged(Shell.Current, EventArgs.Empty);
                };
                return svc;
            });
            builder.Services.AddSingleton<IConfiguration>(configuration);
            builder.Services.AddScoped<IFileService, FileService>();
            var services = builder.Services;
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<UserInfoCardViewModel>());
            services.AddApplication(configuration);
            services.AddPersistence(configuration);
            services.AddExternalCommunication(configuration);
            //   services.AddTransient<CreateOrEditUserPopupViewModel>();

            //  services.AddTransient<CreateOrEditUserPopupView>();

            // resolve DTO dependencies


            services.AddTransient<BackButtonViewModel>(provider =>
            {
                var navigationService = provider.GetService<INavigationService>();
                var popupService = provider.GetService<IPopupService>();
                return new BackButtonViewModel(navigationService, popupService);
            });
            services.AddTransient<BackButtonView>();
            services.AddTransient<PiggyBankPageViewModel>(provider =>
            {
                var userService = provider.GetService<IUserInfoService>();
                var piggyBankService = provider.GetService<IPiggyBankService>();
                var popupService = provider.GetService<IPopupService>();
                var mapper = provider.GetService<IMapper>();
                var transactionService = provider.GetService<ITransactionService>();
                var navService = provider.GetService<INavigationService>();
                var backButtonVm = provider.GetService<BackButtonViewModel>();
                var fileService = provider.GetService<IFileService>();
                return new PiggyBankPageViewModel(backButtonVm, new UserPiggyBankDTO(), new PiggyBankDTO(), new PiggyBankCustomizationDTO(), userService, piggyBankService,popupService, transactionService, mapper, navService, fileService, false);
            });
            services.AddTransient<PiggyBankCardViewModel>();
            services.AddTransient<PiggyBankCardView>();
            services.AddTransientPopup<CreateOrEditUserPopupView, CreateOrEditUserPopupViewModel>();
            services.AddTransientPopup<PreloaderPopupView, PreloaderPopupViewModel>();
            services.AddTransientPopup<ImagePopupView, ImagePopupViewModel>();
            services.AddTransientPopup<PiggyBankInfoPopupView, PiggyBankInfoPopupViewModel>();
            services.AddTransientPopup<TransactionInfoPopupView, TransactionInfoPopupViewModel>();
            services.AddTransientPopup<PrivacyPopupView, PrivacyPopupViewModel>();
            services.AddTransient<FriendlistPopupViewModel>();
            services.AddTransient<FriendlistPopupView>();

            services.AddTransient<PiggyBankPageView>();
            services.AddTransient<UserSettingsPageView>();
            services.AddTransient<UserSettingsPageViewModel>();
            services.AddTransient<UserInfoCardViewModel>();
            services.AddTransient<UserInfoCardView>();
            services.AddTransient<PiggyBanksGalleryPageViewModel>(
                provider =>
            {
                var userService = provider.GetService<IUserInfoService>();
                var piggyBankService = provider.GetService<IPiggyBankService>();
                var popupService = provider.GetService<IPopupService>();
                var mapper = provider.GetService<IMapper>();
                var mediator = provider.GetService<IMediator>();
                var transactionService = provider.GetService<ITransactionService>();
                var integrationService = provider.GetService<IIntegrationService>();
                var navigationService = provider.GetService<INavigationService>();
                var backButtonVm = provider.GetService<BackButtonViewModel>();
                var fileService = provider.GetService<IFileService>();
                return new PiggyBanksGalleryPageViewModel(backButtonVm, mediator, userService, popupService, piggyBankService, mapper, integrationService, navigationService, transactionService, fileService, 0, false, true);
            }
            );
            services.AddTransient<PiggyBanksGalleryPageView>();
            services.AddTransient<PiggyBankMembersPageViewModel>(provider =>
            {
                var userService = provider.GetService<IUserInfoService>();
                var backButtonVm = provider.GetService<BackButtonViewModel>();
                var popupService = provider.GetService<IPopupService>();
                var mapper = provider.GetService<IMapper>();
                var navigationService = provider.GetService<INavigationService>();
                var piggyBankService = provider.GetService<IPiggyBankService>();
                var fileService = provider.GetService<IFileService>();
                return new PiggyBankMembersPageViewModel(new PiggyBankDTO(), new UserDetailsDTO(), backButtonVm, userService, popupService, mapper,navigationService, piggyBankService, fileService);
            });
            services.AddTransient<PiggyBankMembersPageView>();



#if __ANDROID__
            ImageHandler.Mapper.PrependToMapping(nameof(Microsoft.Maui.IImage.Source), (handler, view) => PrependToMappingImageSource(handler, view));
#endif
            var app = builder.Build();

            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "kopilychinternal.db");

            //// Проверка, существует ли файл, и удаление его, если да
            //if (File.Exists(dbPath))
            //{
            //    File.Delete(dbPath); // Удаляем файл, если он существует
            //}

            using (var scope = app.Services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                    var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
                    DbInitializer.Initialize(context);
                var setupWizard = serviceProvider.GetRequiredService<ISetupWizardService>();
                Task.Run(() => setupWizard.ConfigureAsync()).Wait();

            }

            //var sa = dbPath;
            //if (!File.Exists(sa))
            //    throw new FileNotFoundException("Database file not found");

            //// 2. Целевой путь в Downloads
            //var downloadsPath = Android.OS.Environment.GetExternalStoragePublicDirectory(
            //    Android.OS.Environment.DirectoryDownloads)?.AbsolutePath;

            //var destPath = Path.Combine(downloadsPath, "kopilychinternal.db");

            //// 3. Копирование файла
            //File.Copy(sa, destPath, overwrite: true);

            //// 4. Обновление MediaStore (для Android 10+)
            //var mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
            //mediaScanIntent.SetData(Android.Net.Uri.FromFile(new Java.IO.File(destPath)));
            //Platform.CurrentActivity?.SendBroadcast(mediaScanIntent);





            return app;
        }

        public static void PrependToMappingImageSource(IImageHandler handler, Microsoft.Maui.IImage image)
        {
            handler.PlatformView?.Clear();
        }


}
}
