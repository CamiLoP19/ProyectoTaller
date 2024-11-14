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
using System.Windows.Navigation;
using System.Windows.Shapes;
using BLL;
using ENTIDADES;

namespace Proyecto2
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private EmpleadoService empleadoService;
        private UsuarioService usuarioService;
        public MainWindow()
        {
            InitializeComponent();
            usuarioService = new UsuarioService();
            empleadoService = new EmpleadoService();


        }

        private void RoleComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void BtnIniciarSesion_Click(object sender, RoutedEventArgs e)
        {

            string nombreUsuario = txtUsuario.Text;
            string contraseña = txtContraseña.Password;
            string rolSeleccionado = ((ComboBoxItem)cbRoles.SelectedItem).Content.ToString();

            Usuario usuario = usuarioService.IniciarSesion(nombreUsuario, contraseña);

            if (usuario != null && usuario.Rol.ToString() == rolSeleccionado)
            {
                // Abrir la ventana correspondiente al rol
                switch (usuario.Rol)
                {
                    case RolUsuario.Administrador:
                        DuenoWindow dueñoWindow = new DuenoWindow();
                        dueñoWindow.Show();
                        break;
                    case RolUsuario.Empleado:
                        Empleado empleado = empleadoService.ObtenerEmpleadoPorId(usuario.Id);
                        if (empleado != null)
                        {
                            EmpleadoWindow empleadoWindow = new EmpleadoWindow(empleado);
                            empleadoWindow.Show();
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("No se encontró la información del empleado.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        break;
                    case RolUsuario.Cliente:
                        UsuarioWindow usuarioWindow = new UsuarioWindow(usuario);
                        usuarioWindow.Show();
                        break;
                }
                this.Close();
            }
            else
            {
                MessageBox.Show("Credenciales incorrectas o rol no coincide.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void ComboBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox != null)
            {
                comboBox.IsDropDownOpen = true;
                e.Handled = true;
            }
        }

    }

}

      
       
