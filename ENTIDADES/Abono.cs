using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENTIDADES
{
    public class Abono
    {
        public int Id { get; set; }
        public int IdFactura { get; set; }
        public decimal Monto { get; set; }
        public DateTime Fecha { get; set; }
    }

}
