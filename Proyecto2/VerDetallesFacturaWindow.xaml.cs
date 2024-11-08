using System;
using System.Collections.Generic;
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
using ENTIDADES;

namespace Proyecto2
{
    /// <summary>
    /// Lógica de interacción para VerDetallesFacturaWindow.xaml
    /// </summary>
    public partial class VerDetallesFacturaWindow : Window
    {
        public VerDetallesFacturaWindow(Factura factura)
        {
            InitializeComponent();

            // Información de la factura
            txtFacturaInfo.Text = $"Factura N°: {factura.Id} | Fecha: {factura.Fecha.ToShortDateString()}";
            txtClienteInfo.Text = $"Cliente ID: {factura.IdCliente}";
            dgDetallesFactura.ItemsSource = factura.Detalles;

            // Calcular y mostrar el total de la factura
            decimal totalFactura = factura.Detalles.Sum(d => d.Subtotal);
            txtTotalFactura.Text = totalFactura.ToString("C");
        }
    }
}
