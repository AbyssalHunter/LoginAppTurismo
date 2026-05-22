using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace appTurismo.Models
{
    [Table("checkpoints")]
    public class Checkpoint : BaseModel
    {
        // En Supabase la llave primaria se llama id_checkpoint
        [PrimaryKey("id_checkpoint", true)]
        public string IdCheckpoint { get; set; }

        [Column("id_grupo")]
        public string IdGrupo { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; }

        [Column("latitud")]
        public double Latitud { get; set; }

        [Column("longitud")]
        public double Longitud { get; set; }

        [Column("completado")]
        public bool Completado { get; set; }

        [Column("orden")]
        public int Orden { get; set; }
    }
}