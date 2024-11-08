using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BLL;
using ENTIDADES;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Net;
using System.Net.Mail;
using Microsoft.VisualBasic;

namespace Proyecto2
{
    /// <summary>
    /// Lógica de interacción para EmpleadoWindow.xaml
    /// </summary>
    public partial class EmpleadoWindow : Window
    {
        private Empleado empleado;
        private SolicitudService solicitudService;
        private ProductoService productoService;
        private FacturaService facturaService;
        private List<SolicitudServicio> serviciosDisponibles;
        private List<Producto> productosDisponibles;
        private List<DetalleFactura> detallesFactura;
        public EmpleadoWindow(Empleado empleado)
        {
            InitializeComponent();
            this.empleado = empleado;
            solicitudService = new SolicitudService();
            productoService = new ProductoService();
            facturaService = new FacturaService();
            detallesFactura = new List<DetalleFactura>();

            CargarServiciosDisponibles();
            CargarProductosDisponibles();
            ActualizarDataGridDetalles();
        }
        private void ActualizarDataGridDetalles()
        {
            dgDetallesFactura.ItemsSource = null;
            dgDetallesFactura.ItemsSource = detallesFactura;
        }

        private void CargarProductosDisponibles()
        {
            productosDisponibles = productoService.ObtenerProductosDisponibles();
            dgProductosDisponibles.ItemsSource = productosDisponibles;
        }

        private void CargarServiciosDisponibles()
        {
            serviciosDisponibles = solicitudService.ObtenerSolicitudesPendientes();
            dgServiciosDisponibles.ItemsSource = serviciosDisponibles;
        }
        private void BtnTomarServicio_Click(object sender, RoutedEventArgs e)
        {
            SolicitudServicio solicitudSeleccionada = (SolicitudServicio)dgServiciosDisponibles.SelectedItem;
            if (solicitudSeleccionada != null)
            {
                // Cambiar el estado de la solicitud a "EnProceso"
                solicitudSeleccionada.Estado = EstadoSolicitud.EnProceso;
                solicitudSeleccionada.IdEmpleado = empleado.Id; // Asignar el ID del empleado

                string resultado = solicitudService.ActualizarSolicitud(solicitudSeleccionada);
                MessageBox.Show(resultado, "Información", MessageBoxButton.OK, MessageBoxImage.Information);

                // Añadir el servicio a los detalles de la factura
                DetalleFactura detalle = new DetalleFactura
                {
                    IdSolicitudServicio = solicitudSeleccionada.Id,
                    Descripcion = solicitudSeleccionada.Descripcion,
                    Cantidad = 1,
                    PrecioUnitario = solicitudSeleccionada.PrecioEstimado ?? 0
                };

                detallesFactura.Add(detalle);
                ActualizarDataGridDetalles();

                // Recargar los servicios disponibles
                CargarServiciosDisponibles();
            }
            else
            {
                MessageBox.Show("Seleccione un servicio para tomar.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnAñadirProducto_Click(object sender, RoutedEventArgs e)
        {
            Producto productoSeleccionado = (Producto)dgProductosDisponibles.SelectedItem;
            if (productoSeleccionado != null)
            {
                // Pedir la cantidad
                int cantidad = 1; // Puedes implementar una ventana para solicitar la cantidad

                if (cantidad <= 0 || cantidad > productoSeleccionado.Stock)
                {
                    MessageBox.Show("Cantidad inválida o excede el stock disponible.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Añadir el producto a los detalles de la factura
                DetalleFactura detalle = new DetalleFactura
                {
                    IdProducto = productoSeleccionado.Id,
                    Descripcion = productoSeleccionado.Nombre,
                    Cantidad = cantidad,
                    PrecioUnitario = productoSeleccionado.Precio
                };
                detallesFactura.Add(detalle);
                ActualizarDataGridDetalles();

                // Actualizar el stock del producto
                productoSeleccionado.Stock -= cantidad;
                productoService.ActualizarProducto(productoSeleccionado);

                // Recargar los productos disponibles
                CargarProductosDisponibles();
            }
            else
            {
                MessageBox.Show("Seleccione un producto para añadir a la factura.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }


        private void BtnGenerarFactura_Click(object sender, RoutedEventArgs e)

        { 

            if (detallesFactura.Count == 0)
            {
                MessageBox.Show("No hay ítems en la factura.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Obtener el ID del cliente
            int idCliente = ObtenerIdClienteDeDetalles();

            if (idCliente == 0)
            {
                // Ya se muestra un mensaje de error dentro de ObtenerIdClienteDeDetalles
                return;
            }

            // Crear la factura
            Factura nuevaFactura = new Factura
            {
                IdCliente = idCliente,
                IdEmpleado = empleado.Id,
                Fecha = DateTime.Now,
                Total = detallesFactura.Sum(d => d.Subtotal),
                Pagada = false,
                Detalles = detallesFactura
            };

            // Registrar la factura
            string resultado = facturaService.CrearFactura(nuevaFactura);
            MessageBox.Show(resultado, "Información", MessageBoxButton.OK, MessageBoxImage.Information);

            // Limpiar datos
            detallesFactura.Clear();
            ActualizarDataGridDetalles();
        }
        private int ObtenerIdClienteDeDetalles()
        {
            var detalleServicio = detallesFactura.FirstOrDefault(d => d.IdSolicitudServicio.HasValue);
            if (detalleServicio != null)
            {
                int idSolicitudServicio = detalleServicio.IdSolicitudServicio.Value;
                var solicitud = solicitudService.ObtenerSolicitudPorId(idSolicitudServicio);
                if (solicitud != null)
                {
                    return solicitud.IdCliente;
                }
                else
                {
                    MessageBox.Show($"La solicitud de servicio con ID {idSolicitudServicio} no existe.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return 0;
                }
            }
            else
            {
                MessageBox.Show("No hay servicios en los detalles de la factura.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return 0;
            }

        }

        private void BtnCerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            MainWindow loginWindow = new MainWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}
