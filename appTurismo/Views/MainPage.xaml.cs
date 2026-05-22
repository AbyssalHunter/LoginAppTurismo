using appTurismo.ViewModels;

namespace appTurismo.Views;

public partial class MainPage : ContentPage
{
    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Safely pull fresh data when user enters or navigates back to this page layout context
        if (BindingContext is MainViewModel vm)
        {
            if (vm.GetUsersCommand.CanExecute(null))
            {
                vm.GetUsersCommand.Execute(null);
            }
        }
    }
}