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

namespace Proyecto2
{
    /// <summary>
    /// Lógica de interacción para AbonarFacturaWindow.xaml
    /// </summary>
    public partial class AbonarFacturaWindow : Window
    {
        public decimal MontoAbonar { get; private set; }

        public AbonarFacturaWindow(decimal totalFactura)
        {
            InitializeComponent();
            txtMonto.Text = totalFactura.ToString("F2");
            txtMonto.Focus();
        }

        private void BtnAbonar_Click(object sender, RoutedEventArgs e)
        {
            if (decimal.TryParse(txtMonto.Text, out decimal monto) && monto > 0)
            {
                MontoAbonar = monto;
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Ingrese un monto válido.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
