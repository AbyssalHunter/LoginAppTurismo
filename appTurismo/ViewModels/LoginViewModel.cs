using appTurismo.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Controls;

namespace appTurismo.ViewModels
{
    public partial class LoginViewModel : BaseViewModel
    {
        private readonly IUserService _userService;
        private readonly IConnectivity _connectivity;

        [ObservableProperty]
        private string _email = string.Empty;

        [ObservableProperty]
        private string _password = string.Empty;

        public LoginViewModel(IUserService userService, IConnectivity connectivity)
        {
            Title = "Iniciar Sesión";
            _userService = userService;
            _connectivity = connectivity;
        }

        [RelayCommand]
        private async Task LoginAsync()
        {
            if (IsBusy) return;

            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                await Shell.Current.DisplayAlertAsync("Campos Vacíos", "Por favor ingresa tu correo y contraseña.", "OK");
                return;
            }

            if (_connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await Shell.Current.DisplayAlertAsync("Sin Conexión", "Revisa tu conexión a internet.", "OK");
                return;
            }

            IsBusy = true;

            // Evaluamos las credenciales contra el flujo asíncrono
            string? userRole = await _userService.LoginAsync(Email, Password);

            IsBusy = false;

            if (!string.IsNullOrEmpty(userRole))
            {
                // Enrutamiento basado en las especificaciones del rol en minúsculas
                if (userRole == "guia")
                {
                    await Shell.Current.GoToAsync("//AdminPage");
                }
                else if (userRole == "turista")
                {
                    await Shell.Current.GoToAsync("//MainPage");
                }
                else
                {
                    await Shell.Current.DisplayAlertAsync("Error de Acceso", $"El rol '{userRole}' no cuenta con una vista asignada.", "OK");
                }
            }
            else
            {
                await Shell.Current.DisplayAlertAsync("Error", "Correo o contraseña incorrectos. Por favor, intenta de nuevo.", "OK");
            }
        }

        [RelayCommand]
        private async Task GoToRegisterAsync()
        {
            await Shell.Current.GoToAsync("//RegisterPage");
        }
    }
}