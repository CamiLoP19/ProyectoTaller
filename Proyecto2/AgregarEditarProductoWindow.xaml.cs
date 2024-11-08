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
using BLL;
using ENTIDADES;

namespace Proyecto2
{
    /// <summary>
    /// Lógica de interacción para AgregarEditarProductoWindow.xaml
    /// </summary>
    public partial class AgregarEditarProductoWindow : Window
    {
        private ProductoService productoService;
        private Producto producto;
        private bool esNuevo;

        public AgregarEditarProductoWindow()
        {
            InitializeComponent();
            productoService = new ProductoService();
            producto = new Producto();
            esNuevo = true;
        }
        public AgregarEditarProductoWindow(Producto productoExistente)
        {
            InitializeComponent();
            productoService = new ProductoService();
            producto = productoExistente;
            esNuevo = false;
            CargarDatosProducto();
        }
        private void CargarDatosProducto()
        {
            txtNombre.Text = producto.Nombre;
            txtPrecio.Text = producto.Precio.ToString();
            txtStock.Text = producto.Stock.ToString();
        }


        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            producto.Nombre = txtNombre.Text;

            if (decimal.TryParse(txtPrecio.Text, out decimal precio))
            {
                producto.Precio = precio;
            }
            else
            {
                MessageBox.Show("Ingrese un precio válido.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (int.TryParse(txtStock.Text, out int stock))
            {
                producto.Stock = stock;
            }
            else
            {
                MessageBox.Show("Ingrese un stock válido.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string mensaje;
            if (esNuevo)
            {
                mensaje = productoService.AgregarProducto(producto);
            }
            else
            {
                mensaje = productoService.ActualizarProducto(producto);
            }

            MessageBox.Show(mensaje, "Información", MessageBoxButton.OK, MessageBoxImage.Information);
            DialogResult = true;
            Close();
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
    