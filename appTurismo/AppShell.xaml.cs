namespace appTurismo
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("CrearViajePage", typeof(Views.CrearViajePage));
            Routing.RegisterRoute("MapaPage", typeof(Views.MapaPage));
            Routing.RegisterRoute("GestionarCheckpointsPage", typeof(Views.GestionarCheckpointsPage));
        }
    }
}
