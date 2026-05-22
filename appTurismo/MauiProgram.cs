using appTurismo.Services;
using appTurismo.ViewModels;
using appTurismo.DataMapper;
using appTurismo.Views;
using Microsoft.Extensions.Logging;

using SkiaSharp.Views.Maui.Controls.Hosting;

namespace appTurismo
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseSkiaSharp()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            string supabaseUrl = "https://afymvarqqnromdikgwha.supabase.co";
            string supabaseKey = "sb_publishable_ZVc1rTyPdKZWAGqTbu52DQ_TAmgGbHF";

            // Configure Supabase with our custom local persistence preference mapping
            var options = new Supabase.SupabaseOptions
            {
                AutoRefreshToken = true,
                AutoConnectRealtime = true,
                SessionHandler = new appTurismo.Helpers.CustomSupabaseSessionHandler()
            };

            // Instantiate client cleanly without blocking the engine pipeline
            var supabaseClient = new Supabase.Client(supabaseUrl, supabaseKey, options);

            // 2. Base Core System Registrations
            builder.Services.AddSingleton<Supabase.Client>(supabaseClient);
            builder.Services.AddSingleton<IUserService, SupabaseUserService>();
            builder.Services.AddSingleton<IConnectivity>(Connectivity.Current);
            builder.Services.AddSingleton<IStorageService, SupabaseStorageService>();
            builder.Services.AddSingleton<UserMapper>();

            // 3. UI Views & ViewModels registration
            builder.Services.AddSingleton<MainViewModel>();
            builder.Services.AddSingleton<MainPage>();

            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<RegisterViewModel>();
            builder.Services.AddTransient<RegisterPage>();

            builder.Services.AddSingleton<IViajeService, ViajeService>();
            builder.Services.AddTransient<AdminDashViewModel>();
            builder.Services.AddTransient<AdminPage>();

            builder.Services.AddTransient<CrearViajeViewModel>();
            builder.Services.AddTransient<CrearViajePage>();
            builder.Services.AddTransient<MapaPage>();

            builder.Services.AddTransient<GestionarCheckpointsViewModel>();
            builder.Services.AddTransient<GestionarCheckpointsPage>();

            return builder.Build();
        }
    }
}