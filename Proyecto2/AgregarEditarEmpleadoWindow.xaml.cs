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
    /// Lógica de interacción para AgregarEditarEmpleadoWindow.xaml
    /// </summary>
    public partial class AgregarEditarEmpleadoWindow : Window
    {

        private EmpleadoService empleadoService;
        private Empleado empleado;
        private bool esNuevo;
        public AgregarEditarEmpleadoWindow()
        {
            InitializeComponent();
            empleadoService = new EmpleadoService();
            empleado = new Empleado();
            esNuevo = true;

        }
        public AgregarEditarEmpleadoWindow(Empleado empleadoExistente)
        {
            InitializeComponent();
            empleadoService = new EmpleadoService();
            empleado = empleadoExistente;
            esNuevo = false;
            CargarDatosEmpleado();
        }
        private void CargarDatosEmpleado()
        {
            txtNombreUsuario.Text = empleado.NombreUsuario;
            txtNombreUsuario.IsEnabled = false; // No permitir cambiar el nombre de usuario
            txtNombreCompleto.Text = empleado.NombreCompleto;
            txtCorreoElectronico.Text = empleado.CorreoElectronico;
            txtPorcentajeComision.Text = empleado.PorcentajeComision.ToString();
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {

            empleado.NombreUsuario = txtNombreUsuario.Text;
            if (!string.IsNullOrEmpty(txtContraseña.Password))
            {
                empleado.Contraseña = txtContraseña.Password;
            }
            empleado.NombreCompleto = txtNombreCompleto.Text;
            empleado.CorreoElectronico = txtCorreoElectronico.Text;

            if (decimal.TryParse(txtPorcentajeComision.Text, out decimal porcentaje))
            {
                empleado.PorcentajeComision = porcentaje;
            }
            else
            {
                MessageBox.Show("Por favor, ingrese un porcentaje de comisión válido.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            empleado.Rol = RolUsuario.Empleado;

            string mensaje;
            if (esNuevo)
            {
                mensaje = empleadoService.RegistrarEmpleado(empleado);
            }
            else
            {
                mensaje = empleadoService.ActualizarEmpleado(empleado);
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
