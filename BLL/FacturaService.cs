using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DALL;
using ENTIDADES;

namespace BLL
{
    public class FacturaService
    {
        private readonly FacturaRepository facturaRepository;
        private readonly EmpleadoRepository empleadoRepository;
        private readonly ProductoRepository productoRepository;
        private readonly ServicioRepository servicioRepository;

        public FacturaService()
        {
            facturaRepository = new FacturaRepository();
            empleadoRepository = new EmpleadoRepository();
            productoRepository = new ProductoRepository();
            servicioRepository = new ServicioRepository();
        }

        public string CrearFactura(Factura factura)
        {
            if (factura == null)
                return "La factura no puede ser nula.";

            if (factura.Detalles == null || factura.Detalles.Count == 0)
                return "La factura debe tener al menos un detalle.";

            var productoRepository = new ProductoRepository();
            var solicitudService = new SolicitudService();

            factura.Total = 0;
            foreach (var detalle in factura.Detalles)
            {
                if (detalle.IdProducto.HasValue)
                {
                    var producto = productoRepository.ObtenerPorId(detalle.IdProducto.Value);
                    if (producto == null)
                        return $"El producto con ID {detalle.IdProducto.Value} no existe.";

                    if (producto.Stock < detalle.Cantidad)
                        return $"No hay suficiente stock para el producto {producto.Nombre}.";

                    producto.Stock -= detalle.Cantidad;
                    productoRepository.Actualizar(producto);
                }
                else if (detalle.IdSolicitudServicio.HasValue)
                {
                    var solicitudServicio = solicitudService.ObtenerSolicitudPorId(detalle.IdSolicitudServicio.Value);
                    if (solicitudServicio == null)
                        return $"La solicitud de servicio con ID {detalle.IdSolicitudServicio.Value} no existe.";

                    // Opcional: Actualizar el estado de la solicitud a "Completado"
                    solicitudServicio.Estado = EstadoSolicitud.Completada;
                    solicitudService.ActualizarSolicitud(solicitudServicio);
                }

                factura.Total += detalle.Subtotal;
            }

            factura.Fecha = DateTime.Now;
            factura.Pagada = false;

            // Registrar la factura en la base de datos
            return facturaRepository.Insertar(factura);
        }



        public string AbonarFactura(int idFactura, decimal monto)
        {
            return facturaRepository.AbonarFactura(idFactura, monto);
        }

        public Factura ObtenerFacturaPorId(int id)
        {
            return facturaRepository.ObtenerPorId(id);
        }

        public List<Factura> ObtenerFacturas()
        {
            return facturaRepository.ObtenerTodos();
        }

        public List<Factura> ObtenerFacturasPorCliente(int idCliente)
        {
            return facturaRepository.ObtenerFacturasPorCliente(idCliente);
        }
        public Factura ObtenerFacturaConDetalles(int idFactura)
        {
            return facturaRepository.ObtenerFacturaConDetalles(idFactura);
        }

        public List<FacturaConComision> ObtenerFacturasPagadasConComision() {
            return facturaRepository.ObtenerFacturasPagadasConComision();
        }
        // Implementa métodos para actualizar el estado de la factura, pagos, etc.
    }

}
