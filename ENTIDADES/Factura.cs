using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENTIDADES
{
    public class Factura
    {
        public int Id { get; set; }
        public int IdCliente { get; set; }
        public int IdEmpleado { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
        public List<DetalleFactura> Detalles { get; set; }
        public bool Pagada { get; set; }
        public decimal TotalPagado { get; set; }

    }

}
