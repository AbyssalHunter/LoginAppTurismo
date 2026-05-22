using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;

namespace appTurismo.Models.Supabase
{
    [Table("usuarios")]
    public class User : BaseModel
    {
        [PrimaryKey("id_usuario", false)]
        public Guid Id_usuario { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Column("apellido_paterno")]
        public string Apellido_paterno { get; set; } = string.Empty;

        [Column("apellido_materno")]
        public string Apellido_materno { get; set; } = string.Empty;

        [Column("correo_electronico")]
        public string Correo_electronico { get; set; } = string.Empty;

        [Column("telefono")]
        public string Telefono { get; set; } = string.Empty;

        [Column("id_rol")]
        public Guid Id_rol { get; set; }

        [Column("ultima_latitud")]
        public double? Ultima_latitud { get; set; }

        [Column("ultima_longitud")]
        public double? Ultima_longitud { get; set; }

        [Column("ultima_actualizacion")]
        public DateTime? Ultima_actualizacion { get; set; }

        [Column("created_at")]
        public DateTime Created_at { get; set; }
    }
}