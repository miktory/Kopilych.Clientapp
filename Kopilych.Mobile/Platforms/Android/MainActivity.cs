using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Microsoft.Maui.Controls;

namespace Kopilych.Mobile
{
    [Activity(Name="com.miktory.kopilych.MainActivity",Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Обработка Intent
           

            // Другие ваши настройки...
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent); // Не забудьте вызвать базовый метод
            if (intent != null && intent.Action == Intent.ActionView)
            {
                var uri = intent.Data;
                if (uri != null)
                {
                    // Получите параметры из URL
                    var success = uri.GetQueryParameter("success");
                    var accessToken = uri.GetQueryParameter("access_token");
                    var refreshToken = uri.GetQueryParameter("refresh_token");

                    // Здесь вы можете сохранить токены или выполнить другие действия
                    var app = Microsoft.Maui.Controls.Application.Current as App;  // Изменено
                    app.HandleAuthCallback(success, accessToken, refreshToken);
                }
            }

            // Другие ваши настройки...
        }
    }

    [Activity(NoHistory = true, LaunchMode = LaunchMode.SingleTop)]
    [IntentFilter(new[] { Android.Content.Intent.ActionView }, Categories = new[] { Android.Content.Intent.CategoryDefault, Android.Content.Intent.CategoryBrowsable }, DataScheme = "kopilych")]
    public class WebAuthenticatorActivity : WebAuthenticatorCallbackActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            OnNewIntent(Intent);
            // Обработка Intent


            // Другие ваши настройки...
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent); // Не забудьте вызвать базовый метод
            if (intent != null && intent.Action == Intent.ActionView)
            {
                var uri = intent.Data;
                if (uri != null)
                {
                    // Получите параметры из URL
                    var success = uri.GetQueryParameter("success");
                    var accessToken = uri.GetQueryParameter("access_token");
                    var refreshToken = uri.GetQueryParameter("refresh_token");

                    // Здесь вы можете сохранить токены или выполнить другие действия
                    var app = Microsoft.Maui.Controls.Application.Current as App;  // Изменено
                    app.HandleAuthCallback(success, accessToken, refreshToken);

                }
            }

            // Другие ваши настройки...
        }
    }
}
