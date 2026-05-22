using System;
using System.Collections.Generic;

namespace appTurismo.Models
{
    public class GrupoTour
    {
        public string IdTourGroup { get; set; }
        public string IdPaquete { get; set; }
        public string Nombre { get; set; }
        public string GuiaId { get; set; }
        public string Estado { get; set; }
        public DateTime FechaInicio { get; set; }
    }
}