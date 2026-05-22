using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using appTurismo.Models;
using appTurismo.Services;

namespace appTurismo.ViewModels
{
    // Heredamos de BaseViewModel que ya existe en tu proyecto
    public class AdminDashViewModel : BaseViewModel
    {
        private readonly IViajeService _viajeService;

        public ObservableCollection<GrupoTour> ViajesActivos { get; set; }

        public ICommand CargarDashboardCommand { get; }

        public ICommand IrCrearViajeCommand { get; }

        public ICommand VerMapaCommand { get; }

        public ICommand GestionarCheckpointsCommand { get; }

        public AdminDashViewModel(IViajeService viajeService)
        {
            _viajeService = viajeService;
            Title = "Panel de Guía";
            ViajesActivos = new ObservableCollection<GrupoTour>();

            CargarDashboardCommand = new Command(async () => await CargarDashboard());

            IrCrearViajeCommand = new Command(async () => await Shell.Current.GoToAsync("CrearViajePage"));

            VerMapaCommand = new Command<GrupoTour>(async (viaje) =>
            {
                Preferences.Set("ViajeSeleccionado", viaje.IdTourGroup);
                await Shell.Current.GoToAsync("MapaPage");
            });

            GestionarCheckpointsCommand = new Command<GrupoTour>(async (viaje) =>
            {
                Preferences.Set("ViajeSeleccionado", viaje.IdTourGroup);
                await Shell.Current.GoToAsync("GestionarCheckpointsPage");
            });
        }

        private async Task CargarDashboard()
        {
            if (IsBusy) return;
            IsBusy = true;

            var viajes = await _viajeService.GetGuideTripsAsync("GUIA_TEST");

            ViajesActivos.Clear();
            foreach (var viaje in viajes)
            {
                ViajesActivos.Add(viaje);
            }

            IsBusy = false;
        }
    }
}