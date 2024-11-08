using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENTIDADES
{
    public class SolicitudServicio
    {
        public int Id { get; set; }
        public int IdCliente { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public DateTime FechaServicio { get; set; }
        public EstadoSolicitud Estado { get; set; }
        public int? IdEmpleado { get; set; }
        public decimal? PrecioEstimado { get; set; }


    }


}
