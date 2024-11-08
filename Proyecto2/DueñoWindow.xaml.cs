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
using DALL;
using ENTIDADES;

namespace Proyecto2
{
    /// <summary>
    /// Lógica de interacción para DueñoWindow.xaml
    /// </summary>
    public partial class DueñoWindow : Window
    {
        private EmpleadoService empleadoService;
        private ProductoService productoService;
        private List<Empleado> listaEmpleados;
        private List<Producto> listaProductos;
        private FacturaService facturaService;

       



        public DueñoWindow()
        {
            InitializeComponent();
            empleadoService = new EmpleadoService();
            productoService = new ProductoService();
            facturaService = new FacturaService();

            CargarGanancias();
            CargarEmpleados();
            CargarProductos();

        }
      
        private void CargarGanancias()
        {
            // Obtener las facturas pagadas con información de comisión
            var facturasConComision = facturaService.ObtenerFacturasPagadasConComision();

            // Calcular el total de ventas
            decimal totalVentas = facturasConComision.Sum(f => f.Total);

            // Calcular el total de comisiones pagadas
            decimal totalComisiones = facturasConComision.Sum(f => f.ComisionEmpleado);

            // Calcular las ganancias netas del dueño
            decimal gananciasNetas = totalVentas - totalComisiones;

            // Mostrar las ganancias netas en el label
            lblTotalGanancias.Content = $"Ganancias Netas del Dueño: {gananciasNetas:C}";

            // Opcional: Mostrar el total de ventas y total de comisiones
            lblTotalVentas.Content = $"Total de Ventas: {totalVentas:C}";
            lblTotalComisiones.Content = $"Total de Comisiones Pagadas: {totalComisiones:C}";

            // Mostrar las facturas en el DataGrid
            dgFacturasPagadas.ItemsSource = facturasConComision;
        }

        private void CargarProductos()
        {
            listaProductos = productoService.ObtenerProductos();
            dgProductos.ItemsSource = listaProductos;
        }

        private void CargarEmpleados()
        {
            listaEmpleados = empleadoService.ObtenerEmpleados();
            dgEmpleados.ItemsSource = listaEmpleados;
        }

        private void BtnAgregarEmpleado_Click(object sender, RoutedEventArgs e)
        {
            AgregarEditarEmpleadoWindow ventana = new AgregarEditarEmpleadoWindow();
            if (ventana.ShowDialog() == true)
            {
                CargarEmpleados();
            }
        }

        private void BtnEditarEmpleado_Click(object sender, RoutedEventArgs e)
        {
            Empleado empleadoSeleccionado = (Empleado)dgEmpleados.SelectedItem;
            if (empleadoSeleccionado != null)
            {
                AgregarEditarEmpleadoWindow ventana = new AgregarEditarEmpleadoWindow(empleadoSeleccionado);
                if (ventana.ShowDialog() == true)
                {
                    CargarEmpleados();
                }
            }
            else
            {
                MessageBox.Show("Seleccione un empleado para editar.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnEliminarEmpleado_Click(object sender, RoutedEventArgs e)
        {
            Empleado empleadoSeleccionado = (Empleado)dgEmpleados.SelectedItem;
            if (empleadoSeleccionado != null)
            {
                MessageBoxResult resultado = MessageBox.Show($"¿Está seguro de eliminar al empleado {empleadoSeleccionado.NombreCompleto}?", "Confirmación", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (resultado == MessageBoxResult.Yes)
                {
                    string mensaje = empleadoService.EliminarEmpleado(empleadoSeleccionado.Id);
                    MessageBox.Show(mensaje, "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                    CargarEmpleados();
                }
            }
            else
            {
                MessageBox.Show("Seleccione un empleado para eliminar.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnAgregarProducto_Click(object sender, RoutedEventArgs e)
        {
            AgregarEditarProductoWindow ventana = new AgregarEditarProductoWindow();
            if (ventana.ShowDialog() == true)
            {
                CargarProductos();
            }
        }

        private void BtnEditarProducto_Click(object sender, RoutedEventArgs e)
        {
            Producto productoSeleccionado = (Producto)dgProductos.SelectedItem;
            if (productoSeleccionado != null)
            {
                AgregarEditarProductoWindow ventana = new AgregarEditarProductoWindow(productoSeleccionado);
                if (ventana.ShowDialog() == true)
                {
                    CargarProductos();
                }
            }
            else
            {
                MessageBox.Show("Seleccione un producto para editar.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnEliminarProducto_Click(object sender, RoutedEventArgs e)
        {
            Producto productoSeleccionado = (Producto)dgProductos.SelectedItem;
            if (productoSeleccionado != null)
            {
                MessageBoxResult resultado = MessageBox.Show($"¿Está seguro de eliminar el producto {productoSeleccionado.Nombre}?", "Confirmación", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (resultado == MessageBoxResult.Yes)
                {
                    string mensaje = productoService.EliminarProducto(productoSeleccionado.Id);
                    MessageBox.Show(mensaje, "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                    CargarProductos();
                }
            }
            else
            {
                MessageBox.Show("Seleccione un producto para eliminar.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
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
