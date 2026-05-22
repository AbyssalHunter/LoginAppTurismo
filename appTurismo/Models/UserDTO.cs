using CommunityToolkit.Mvvm.ComponentModel;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace appTurismo.Models;

[Table("usuarios")]
public partial class UserDTO : ObservableObject
{
    public Guid IdUsuario { get; set; }

    [ObservableProperty]
    private string _nombre;

    [ObservableProperty]
    private string _apellidoPaterno;

    [ObservableProperty]
    private string _apellidoMaterno;

    [ObservableProperty]
    private string _correoElectronico;

    [ObservableProperty]
    private string _telefono;

    public Guid IdRol { get; set; }

    public double? UltimaLatitud { get; set; }

    public double? UltimaLongitud { get; set; }

    public DateTime? UltimaActualizacion { get; set; }

    public DateTime CreatedAt { get; set; }
}