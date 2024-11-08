using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENTIDADES
{
    public class Empleado : Usuario
    {
        public string NombreCompleto { get; set; }
        public decimal PorcentajeComision { get; set; } = 0.60m; // 60% por defecto
    }

}
