using appTurismo.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace appTurismo.Services
{
    public interface IViajeService
    {
        Task<List<GrupoTour>> GetGuideTripsAsync(string guiaId);
        Task CreateTripAsync(GrupoTour grupo, List<Checkpoint> puntos);
    }

    public class ViajeService : IViajeService
    {
        private readonly Supabase.Client _supabaseClient;

        public ViajeService(Supabase.Client supabaseClient)
        {
            _supabaseClient = supabaseClient;
        }

        // 1. AHORA LEEMOS LOS VIAJES REALES DE SUPABASE
        public async Task<List<GrupoTour>> GetGuideTripsAsync(string guiaId)
        {
            var lista = new List<GrupoTour>();
            try
            {
                var respuesta = await _supabaseClient.From<Grupo>().Get();

                foreach (var g in respuesta.Models)
                {
                    lista.Add(new GrupoTour
                    {
                        IdTourGroup = g.IdTourGroup,
                        Nombre = g.Nombre ?? "Viaje Sin Nombre", // Si es NULL, pone esto
                        Estado = g.Estado,
                        FechaInicio = g.FechaInicio
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al leer viajes: {ex.Message}");
            }
            return lista;
        }

        // 2. AHORA GUARDAMOS CON EL NOMBRE Y EL UUID CORRECTO
        public async Task CreateTripAsync(GrupoTour grupo, List<Checkpoint> puntos)
        {
            try
            {
                var nuevoGrupoSupabase = new Grupo
                {
                    IdTourGroup = grupo.IdTourGroup, // Como pusimos 'true', Supabase respetará este ID
                    Nombre = grupo.Nombre,
                    Estado = "Activo",
                    FechaInicio = grupo.FechaInicio
                };

                // 1. Guardamos el Grupo
                await _supabaseClient.From<Grupo>().Insert(nuevoGrupoSupabase);

                // 2. Guardamos los Checkpoints vinculados a ese MISMO ID
                if (puntos != null && puntos.Count > 0)
                {
                    int orden = 1;
                    foreach (var p in puntos)
                    {
                        p.IdCheckpoint = Guid.NewGuid().ToString(); // Generamos el ID del punto
                        p.IdGrupo = grupo.IdTourGroup;              // Lo enlazamos fuertemente al Grupo
                        p.Orden = orden;

                        try
                        {
                            await _supabaseClient.From<Checkpoint>().Insert(p);
                        }
                        catch (Exception exPunto)
                        {
                            System.Diagnostics.Debug.WriteLine($"[ERROR PUNTO]: {exPunto.Message}");
                        }
                        orden++;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ERROR GRUPO]: {ex.Message}");
            }
        }
    }
}