using appTurismo.Models;
using Mapsui.UI.Maui;
using Mapsui.Projections;
using Microsoft.Maui.Devices.Sensors;

namespace appTurismo.Views;

public partial class MapaPage : ContentPage
{
    private readonly Supabase.Client _supabaseClient;

    // Inyectamos Supabase para poder leer la base de datos
    public MapaPage(Supabase.Client supabaseClient)
    {
        InitializeComponent();
        _supabaseClient = supabaseClient;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (mapView.Map == null) mapView.Map = new Mapsui.Map();
        mapView.Map.Layers.Clear();
        mapView.Map.Layers.Add(Mapsui.Tiling.OpenStreetMap.CreateTileLayer());
        mapView.Pins.Clear(); // Limpiamos el mapa

        // 1. LEEMOS EL ID DEL VIAJE QUE SELECCIONASTE
        var idViaje = Preferences.Get("ViajeSeleccionado", "");

        if (!string.IsNullOrEmpty(idViaje))
        {
            try
            {
                // 2. DESCARGAMOS LOS CHECKPOINTS DE ESE VIAJE DESDE SUPABASE
                var respuesta = await _supabaseClient.From<Checkpoint>().Where(c => c.IdGrupo == idViaje).Get();

                // 3. DIBUJAMOS LOS PINES REALES EN EL MAPA
                foreach (var punto in respuesta.Models)
                {
                    var pin = new Pin(mapView)
                    {
                        Label = punto.Nombre,
                        Position = new Position(punto.Latitud, punto.Longitud),
                        Type = PinType.Pin,
                        // Si ya pasaste lista, el pin se vuelve Verde. Si no, es Rojo.
                        Color = punto.Completado ? Colors.Green : Colors.Red
                    };
                    mapView.Pins.Add(pin);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al traer checkpoints: {ex.Message}");
            }
        }

        // 4. Centramos la cámara y activamos GPS
        var (x, y) = SphericalMercator.FromLonLat(-99.1332, 19.4326);
        mapView.Map.Navigator.CenterOn(new Mapsui.MPoint(x, y));
        mapView.Refresh();

        await ActivarGPS(); // (Usa la misma función ActivarGPS que ya tienes en CrearViajePage)
    }

    private async Task ActivarGPS()
    {
        try
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted) status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

            if (status == PermissionStatus.Granted)
            {
                mapView.MyLocationEnabled = true;
                var location = await Geolocation.Default.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(10)));
                if (location != null)
                {
                    mapView.MyLocationLayer.UpdateMyLocation(new Position(location.Latitude, location.Longitude));
                    var (miX, miY) = SphericalMercator.FromLonLat(location.Longitude, location.Latitude);
                    mapView.Map.Navigator.CenterOn(new Mapsui.MPoint(miX, miY));
                    mapView.Map.Navigator.ZoomTo(10);
                    mapView.Refresh();
                }
            }
        }
        catch (Exception) { /* Ignorar errores de GPS si falla */ }
    }
}