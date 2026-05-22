using appTurismo.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace appTurismo.ViewModels
{
    public partial class MainViewModel : BaseViewModel
    {
        public ObservableCollection<Models.UserDTO> Contacts { get; } = new();
        private readonly IUserService _userService;
        private readonly IConnectivity _connectivity;

        public MainViewModel(IUserService userService, IConnectivity connectivity, IStorageService imageStorageService)
        {
            Title = "Users";
            _userService = userService;
            _connectivity = connectivity;

            // REMOVED: GetUsersCommand.Execute(null); to prevent initialization database query crashes
        }

        [ObservableProperty]
        private bool _isRefreshing;

        [RelayCommand]
        async Task ShowMessageAsync()
        {
            await Shell.Current.DisplayAlertAsync("No connectivity!",
                $"Please check internet and try again.", "OK");
        }

        [RelayCommand]
        async Task GoToContactDetailsAsync(Models.UserDTO? user = null)
        {
            await Shell.Current.GoToAsync(nameof(Views.UserPage), true, new Dictionary<string, object>
            {
                { nameof(Models.UserDTO), user ?? new Models.UserDTO() },
                { "isUpdate", user != null }
            });
        }

        [RelayCommand]
        async Task GetUsersAsync()
        {
            if (IsBusy)
                return;

            try
            {
                if (_connectivity.NetworkAccess != NetworkAccess.Internet)
                {
                    await Shell.Current.DisplayAlertAsync("No connectivity!",
                        "Please check internet and try again.", "OK");
                    return;
                }

                IsBusy = true;
                IsRefreshing = true;

                if (Contacts.Count != 0)
                    Contacts.Clear();

                var users = await _userService.GetUsersAsync();

                foreach (var user in users)
                {
                    Contacts.Add(user);
                }

                await Shell.Current.DisplayAlertAsync("Success",
                    $"Loaded {users.Count} users", "OK");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to get users: {ex.Message}");
                await Shell.Current.DisplayAlertAsync("Error!",
                    $"Failed to load users: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
                IsRefreshing = false;
            }
        }
    }
}