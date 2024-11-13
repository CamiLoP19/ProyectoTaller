using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BLL;
using ENTIDADES;
using iTextSharp.text.pdf;
using iTextSharp.text;

namespace Proyecto2
{
    /// <summary>
    /// Lógica de interacción para UsuarioWindow.xaml
    /// </summary>
    public partial class UsuarioWindow : Window
    {
        private Usuario usuario;
        private FacturaService facturaService;
        private SolicitudService solicitudService;
        private List<Factura> listaFacturas;
        public UsuarioWindow(Usuario usuario)
        {
            InitializeComponent();
            this.usuario = usuario;
            facturaService = new FacturaService();
            solicitudService = new SolicitudService();
            CargarFacturas();
        }
        private void CargarFacturas()
        {
            listaFacturas = facturaService.ObtenerFacturasPorCliente(usuario.Id);
            dgFacturas.ItemsSource = listaFacturas;
        }


        private void BtnGenerarServicio_Click(object sender, RoutedEventArgs e)
        {
            string descripcion = txtDescripcionServicio.Text;
            DateTime? fechaServicio = dpFechaServicio.SelectedDate;

            if (string.IsNullOrWhiteSpace(descripcion))
            {
                MessageBox.Show("Ingrese la descripción del servicio.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!fechaServicio.HasValue)
            {
                MessageBox.Show("Seleccione la fecha del servicio.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Crear la solicitud de servicio
            SolicitudServicio solicitud = new SolicitudServicio
            {
                IdCliente = usuario.Id,
                Descripcion = descripcion,
                FechaSolicitud = DateTime.Now,
                FechaServicio = fechaServicio.Value,
                Estado = EstadoSolicitud.Pendiente
            };

            string resultado = solicitudService.CrearSolicitud(solicitud);
            MessageBox.Show(resultado, "Información", MessageBoxButton.OK, MessageBoxImage.Information);

            // Limpiar campos
            txtDescripcionServicio.Text = "";
            dpFechaServicio.SelectedDate = null;

        }

        private void BtnVerDetalles_Click(object sender, RoutedEventArgs e)
        {
            Factura facturaSeleccionada = (Factura)dgFacturas.SelectedItem;
            if (facturaSeleccionada != null)
            {
                // Obtener los detalles de la factura
                facturaSeleccionada = facturaService.ObtenerFacturaConDetalles(facturaSeleccionada.Id);
                VerDetallesFacturaWindow ventanaDetalles = new VerDetallesFacturaWindow(facturaSeleccionada);
                ventanaDetalles.Owner = this;
                ventanaDetalles.ShowDialog();
            }
            else
            {
                MessageBox.Show("Seleccione una factura para ver los detalles.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnAbonarFactura_Click(object sender, RoutedEventArgs e)
        {
            Factura facturaSeleccionada = (Factura)dgFacturas.SelectedItem;
            if (facturaSeleccionada != null)
            {
                // Obtener la factura actualizada
                facturaSeleccionada = facturaService.ObtenerFacturaConDetalles(facturaSeleccionada.Id);

                // Abrir ventana para abonar
                AbonarFacturaWindow abonarWindow = new AbonarFacturaWindow(facturaSeleccionada.Total);
                abonarWindow.Owner = this;
                if (abonarWindow.ShowDialog() == true)
                {
                    decimal montoAbonar = abonarWindow.MontoAbonar;

                    // registrar el abono
                    string resultado = facturaService.AbonarFactura(facturaSeleccionada.Id, montoAbonar);
                    MessageBox.Show(resultado, "Información", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Recargar facturas
                    CargarFacturas();
                }
            }
            else
            {
                MessageBox.Show("Seleccione una factura para abonar.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnDescargarPDF_Click(object sender, RoutedEventArgs e)
        {
            Factura facturaSeleccionada = (Factura)dgFacturas.SelectedItem;
            if (facturaSeleccionada != null)
            {
                // Obtener la factura con detalles
                facturaSeleccionada = facturaService.ObtenerFacturaConDetalles(facturaSeleccionada.Id);

                // Generar el PDF
                string rutaArchivo = GenerarFacturaPDF(facturaSeleccionada);

                MessageBox.Show($"Factura descargada en: {rutaArchivo}", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Seleccione una factura para descargar.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private string GenerarFacturaPDF(Factura factura)
        {
            Document documento = new Document();
            string nombreArchivo = $"Factura_{factura.Id}.pdf";
            string rutaArchivo = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), nombreArchivo);

            PdfWriter.GetInstance(documento, new FileStream(rutaArchivo, FileMode.Create));
            documento.Open();

            // Agregar contenido al PDF
            documento.Add(new iTextSharp.text.Paragraph($"Factura N°: {factura.Id}"));
            documento.Add(new iTextSharp.text.Paragraph($"Fecha: {factura.Fecha.ToShortDateString()}"));
            documento.Add(new iTextSharp.text.Paragraph($"Total: {factura.Total.ToString("C")}"));
            documento.Add(new iTextSharp.text.Paragraph(" "));
            documento.Add(new iTextSharp.text.Paragraph("Detalles:"));

            // Agregar una tabla con los detalles de la factura
            PdfPTable tabla = new PdfPTable(4);
            tabla.WidthPercentage = 100;
            tabla.AddCell("Descripción");
            tabla.AddCell("Cantidad");
            tabla.AddCell("Precio Unitario");
            tabla.AddCell("Subtotal");

            foreach (var detalle in factura.Detalles)
            {
                tabla.AddCell(detalle.Descripcion);
                tabla.AddCell(detalle.Cantidad.ToString());
                tabla.AddCell(detalle.PrecioUnitario.ToString("C"));
                tabla.AddCell(detalle.Subtotal.ToString("C"));
            }

            documento.Add(tabla);
            documento.Close();

            return rutaArchivo;
        }

        private void BtnCerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            MainWindow loginWindow = new MainWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}
