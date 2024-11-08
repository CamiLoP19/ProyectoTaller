using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENTIDADES
{
    public class FacturaConComision
    {
        public int Id { get; set; }
        public decimal Total { get; set; }
        public int IdEmpleado { get; set; }
        public decimal PorcentajeComision { get; set; }
        public decimal ComisionEmpleado => Total * (PorcentajeComision / 100);
    }
}
