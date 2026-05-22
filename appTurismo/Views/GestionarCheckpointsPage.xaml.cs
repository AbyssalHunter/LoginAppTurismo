using appTurismo.ViewModels;

namespace appTurismo.Views;

public partial class GestionarCheckpointsPage : ContentPage
{
    private readonly GestionarCheckpointsViewModel _viewModel;

    public GestionarCheckpointsPage(GestionarCheckpointsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    // Leemos el ID del viaje desde la ruta
    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        // Asumimos que le pasaremos el ID en la ruta como "?grupoId=xxx"
        // Para la presentación rápida de mañana, podemos usar Preferences (como una variable global)
        var idViajeActual = Preferences.Get("ViajeSeleccionado", "");
        if (!string.IsNullOrEmpty(idViajeActual))
        {
            await _viewModel.CargarCheckpoints(idViajeActual);
        }
    }
}