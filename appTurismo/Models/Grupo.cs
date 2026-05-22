using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;

namespace appTurismo.Models
{
    // Le decimos exactamente cómo se llama la tabla en Supabase
    [Table("grupos")]
    public class Grupo : BaseModel
    {
        [PrimaryKey("id_tour_group", true)]
        public string IdTourGroup { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; }

        [Column("id_paquete")]
        public string IdPaquete { get; set; }

        [Column("guia_id")]
        public string GuiaId { get; set; }

        [Column("estado")]
        public string Estado { get; set; }

        [Column("fecha_inicio")]
        public DateTime FechaInicio { get; set; }
    }
}