using appTurismo.ViewModels;

namespace appTurismo.Views;

public partial class AdminPage : ContentPage
{
    public AdminPage(AdminDashViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is AdminDashViewModel vm)
            vm.CargarDashboardCommand.Execute(null);
    }
}