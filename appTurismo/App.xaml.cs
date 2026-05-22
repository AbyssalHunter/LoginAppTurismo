using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.Maui.Controls;
using System.Collections.Generic;

namespace appTurismo
{
    public partial class App : Application
    {
        private readonly Supabase.Client _supabaseClient;

        public App(Supabase.Client supabaseClient)
        {
            InitializeComponent();
            _supabaseClient = supabaseClient;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }

        protected override async void OnStart()
        {
            base.OnStart();

            try
            {
                await _supabaseClient.InitializeAsync();

                if (_supabaseClient.Auth.CurrentSession != null)
                {
                    Guid userId = Guid.Parse(_supabaseClient.Auth.CurrentSession.User.Id);

                    // 💡 CAMBIO AQUÍ TAMBIÉN: Usamos el Rpc fuertemente tipado
                    var userRole = await _supabaseClient.Rpc<string>("get_user_role", new Dictionary<string, object>
            {
                { "user_uuid", userId }
            });

                    string? cleanRole = userRole?.ToLower().Trim();

                    // Redirección segura según las reglas de tu negocio
                    if (cleanRole == "guia")
                    {
                        await Shell.Current.GoToAsync("//AdminPage");
                    }
                    else
                    {
                        await Shell.Current.GoToAsync("//MainPage");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Supabase Startup Error]: {ex.Message}");
            }
        }
    }
}