using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using appTurismo.Models;
using appTurismo.Services;

namespace appTurismo.ViewModels
{
    public class CrearViajeViewModel : BaseViewModel
    {
        private readonly IViajeService _viajeService;

        private string _nombrePaquete;
        public string NombrePaquete
        {
            get => _nombrePaquete;
            set { _nombrePaquete = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Checkpoint> CheckpointsNuevos { get; set; }
        public ICommand GuardarViajeCommand { get; }

        public CrearViajeViewModel(IViajeService viajeService)
        {
            _viajeService = viajeService;
            Title = "Nuevo Viaje";
            CheckpointsNuevos = new ObservableCollection<Checkpoint>();
            GuardarViajeCommand = new Command(async () => await GuardarViaje());
        }

        private async Task GuardarViaje()
        {
            if (string.IsNullOrWhiteSpace(NombrePaquete)) return;

            // Generamos el código matemático que Supabase exige
            var uuidReal = Guid.NewGuid().ToString();

            var nuevoViaje = new GrupoTour
            {
                IdTourGroup = uuidReal,
                Nombre = NombrePaquete, // Guardamos el nombre "Playa" en su nueva columna
                FechaInicio = DateTime.Now
            };

            await _viajeService.CreateTripAsync(nuevoViaje, CheckpointsNuevos.ToList());
            await Shell.Current.GoToAsync("..");
        }
    }
}