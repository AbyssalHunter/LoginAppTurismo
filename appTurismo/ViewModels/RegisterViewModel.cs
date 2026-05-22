using appTurismo.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace appTurismo.ViewModels
{
    public partial class RegisterViewModel : BaseViewModel
    {
        private readonly IUserService _userService;
        private readonly IConnectivity _connectivity;

        [ObservableProperty] private string _email = string.Empty;
        [ObservableProperty] private string _password = string.Empty;
        [ObservableProperty] private string _nombre = string.Empty;
        [ObservableProperty] private string _apellidoPaterno = string.Empty;
        [ObservableProperty] private string _apellidoMaterno = string.Empty;
        [ObservableProperty] private string _telefono = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(TuristaButtonColor))]
        [NotifyPropertyChangedFor(nameof(GuiaButtonColor))]
        private string _selectedRole = "turista";

        public string TuristaButtonColor => SelectedRole == "turista" ? "DarkCyan" : "LightGray";
        public string GuiaButtonColor => SelectedRole == "guia" ? "DarkCyan" : "LightGray";

        public RegisterViewModel(IUserService userService, IConnectivity connectivity)
        {
            Title = "Crear Cuenta";
            _userService = userService;
            _connectivity = connectivity;
        }

        [RelayCommand]
        private void SelectRole(string role)
        {
            if (string.IsNullOrWhiteSpace(role)) return;

            var lowerRole = role.ToLower().Trim();
            if (lowerRole == "turista" || lowerRole == "guia")
            {
                SelectedRole = lowerRole;
            }
        }

        [RelayCommand]
        private async Task RegisterAsync()
        {
            if (IsBusy) return;

            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(Nombre))
            {
                await Shell.Current.DisplayAlertAsync("Faltan Datos", "Email, Password, y Nombre son obligatorios.", "OK");
                return;
            }

            if (_connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await Shell.Current.DisplayAlertAsync("Sin Conexión", "No hay acceso a internet.", "OK");
                return;
            }

            IsBusy = true;

            var nuevoUsuario = new Models.Supabase.User
            {
                Nombre = Nombre,
                Apellido_paterno = ApellidoPaterno,
                Apellido_materno = ApellidoMaterno,
                Telefono = Telefono
            };

            // Sends down authentication credentials alongside targeted profile information and selection variables
            bool success = await _userService.RegisterWithRoleAsync(Email, Password, nuevoUsuario, SelectedRole);
            IsBusy = false;

            if (success)
            {
                await Shell.Current.DisplayAlertAsync("Éxito", "Usuario registrado con éxito. Confirma tu correo para ingresar.", "OK");
                await Shell.Current.GoToAsync("//LoginPage");
            }
            else
            {
                await Shell.Current.DisplayAlertAsync("Error", "No se pudo registrar el usuario. Revisa las credenciales.", "OK");
            }
        }

        [RelayCommand]
        private async Task GoToLoginAsync()
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}