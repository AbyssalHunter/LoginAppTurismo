using appTurismo.ViewModels;
using appTurismo.Models;
using Mapsui.UI.Maui;
using Mapsui.Projections;
using Microsoft.Maui.Devices.Sensors;

namespace appTurismo.Views;

public partial class CrearViajePage : ContentPage
{
    private CrearViajeViewModel _viewModel;
    private int _contadorPuntos = 1;

    public CrearViajePage(CrearViajeViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (mapView.Map == null)
        {
            mapView.Map = new Mapsui.Map();
            // ¡Ya no conectamos MapClicked, ahora usamos el botón!
        }

        mapView.Map.Layers.Clear();
        mapView.Map.Layers.Add(Mapsui.Tiling.OpenStreetMap.CreateTileLayer());

        // Activamos tu GPS para centrar el mapa al abrir la pantalla
        try
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted) status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

            if (status == PermissionStatus.Granted)
            {
                mapView.MyLocationEnabled = true;
                var location = await Geolocation.Default.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.Best));

                if (location != null)
                {
                    mapView.MyLocationLayer.UpdateMyLocation(new Position(location.Latitude, location.Longitude));
                    var (miX, miY) = SphericalMercator.FromLonLat(location.Longitude, location.Latitude);
                    mapView.Map.Navigator.CenterOn(new Mapsui.MPoint(miX, miY));
                    mapView.Map.Navigator.ZoomTo(10);
                }
            }
        }
        catch { }

        mapView.Refresh();
    }

    // ¡NUEVA FUNCIÓN! Agrega el punto exactamente donde está el centro de la pantalla
    private void BtnAgregarCentro_Clicked(object sender, EventArgs e)
    {
        // 1. Leemos exactamente dónde está centrada la cámara del mapa en este instante
        var centroX = mapView.Map.Navigator.Viewport.CenterX;
        var centroY = mapView.Map.Navigator.Viewport.CenterY;

        // 2. Lo convertimos a Coordenadas GPS (Latitud/Longitud) desempaquetando directamente
        var (longitud, latitud) = SphericalMercator.ToLonLat(centroX, centroY);

        // 3. Lógica para el "Inicio de Ruta" que pediste
        string nombrePunto;
        if (_contadorPuntos == 1)
            nombrePunto = "Inicio de Ruta";
        else
            nombrePunto = $"Punto {_contadorPuntos}";

        // 4. Dibujamos el Pin rojo para que el Guía vea que sí se agregó
        var nuevoPin = new Pin(mapView)
        {
            Label = nombrePunto,
            Position = new Position(latitud, longitud),
            Type = PinType.Pin,
            Color = Colors.Red
        };

        mapView.Pins.Add(nuevoPin);
        mapView.Refresh();

        // 5. Lo guardamos en la memoria para que se envíe a Supabase al darle Guardar
        _viewModel.CheckpointsNuevos.Add(new Checkpoint
        {
            Nombre = nombrePunto,
            Latitud = latitud,
            Longitud = longitud,
            Completado = false
        });

        _contadorPuntos++;

        // Un mensajito opcional para confirmar
        DisplayAlert("Éxito", $"'{nombrePunto}' agregado a la ruta.", "OK");
    }
}