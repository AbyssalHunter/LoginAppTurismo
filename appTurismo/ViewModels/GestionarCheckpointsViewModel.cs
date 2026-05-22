using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using appTurismo.Models;
using appTurismo.Services; // Asumimos que tienes un servicio o lo leeremos directo de Supabase

namespace appTurismo.ViewModels
{
    public class GestionarCheckpointsViewModel : BaseViewModel
    {
        private readonly Supabase.Client _supabaseClient;
        private string _grupoId;

        public ObservableCollection<Checkpoint> ListaCheckpoints { get; set; }

        public ICommand MarcarCompletadoCommand { get; }

        public GestionarCheckpointsViewModel(Supabase.Client supabaseClient)
        {
            _supabaseClient = supabaseClient;
            Title = "Control de Checkpoints";
            ListaCheckpoints = new ObservableCollection<Checkpoint>();

            // Comando para cambiar el estado del checkpoint en Supabase
            MarcarCompletadoCommand = new Command<Checkpoint>(async (cp) => await CompletarCheckpoint(cp));
        }

        // Esta función se llamará cuando la pantalla se abra para cargar los puntos de ese viaje específico
        public async Task CargarCheckpoints(string grupoId)
        {
            if (string.IsNullOrEmpty(grupoId)) return;
            _grupoId = grupoId;
            IsBusy = true;

            try
            {
                // Vamos a la nube y traemos solo los puntos de ESTE viaje ordenados por número
                var respuesta = await _supabaseClient.From<Checkpoint>()
                                                     .Where(c => c.IdGrupo == _grupoId)
                                                     .Order(c => c.Orden, Supabase.Postgrest.Constants.Ordering.Ascending)
                                                     .Get();

                ListaCheckpoints.Clear();
                foreach (var checkpoint in respuesta.Models)
                {
                    ListaCheckpoints.Add(checkpoint);
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al cargar checkpoints: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task CompletarCheckpoint(Checkpoint cp)
        {
            if (cp == null || cp.Completado) return;

            cp.Completado = true; // Lo marcamos a nivel local para que la UI cambie de color

            try
            {
                // Le decimos a Supabase que actualice solo este punto
                await _supabaseClient.From<Checkpoint>()
                                     .Where(x => x.IdCheckpoint == cp.IdCheckpoint)
                                     .Set(x => x.Completado, true)
                                     .Update();

                // Refrescamos la lista para que el cambio se note en pantalla
                await CargarCheckpoints(_grupoId);
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al actualizar: {ex.Message}");
                cp.Completado = false; // Revertimos si hay error
            }
        }
    }
}