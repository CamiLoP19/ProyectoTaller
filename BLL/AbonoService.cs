using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DALL;
using ENTIDADES;

namespace BLL
{
    public class AbonoService
    {
        private readonly AbonoRepository abonoRepository;
        private readonly FacturaRepository facturaRepository;

        public AbonoService()
        {
            abonoRepository = new AbonoRepository();
            facturaRepository = new FacturaRepository();
        }

        public string RegistrarAbono(Abono abono)
        {
            // Validaciones de negocio
            if (abono == null)
                return "El abono no puede ser nulo.";

            if (abono.Monto <= 0)
                return "El monto del abono debe ser mayor a cero.";

            var factura = facturaRepository.ObtenerPorId(abono.IdFactura);
            if (factura == null)
                return $"La factura con ID {abono.IdFactura} no existe.";

            // Calcular el total de abonos previos
            var abonosPrevios = abonoRepository.ObtenerPorFacturaId(abono.IdFactura);
            decimal totalAbonado = 0;
            foreach (var ab in abonosPrevios)
            {
                totalAbonado += ab.Monto;
            }

            // Verificar que el abono no exceda el total de la factura
            if (totalAbonado + abono.Monto > factura.Total)
                return "El monto abonado excede el total de la factura.";

            // Registrar el abono
            abono.Fecha = DateTime.Now;
            var resultado = abonoRepository.Insertar(abono);

            // Actualizar estado de la factura si se ha completado el pago
            if (totalAbonado + abono.Monto == factura.Total)
            {
                factura.Pagada = true;
                // Aquí podrías implementar un método para actualizar la factura
            }

            return resultado;
        }

        public List<Abono> ObtenerAbonosPorFactura(int idFactura)
        {
            return abonoRepository.ObtenerPorFacturaId(idFactura);
        }
    }


}