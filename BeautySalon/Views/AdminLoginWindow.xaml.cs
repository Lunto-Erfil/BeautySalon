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

namespace BeautySalon.Views
{
    public partial class AdminLoginWindow : Window
    {
        private const string AdminCode = "0000";

        public AdminLoginWindow() => InitializeComponent();

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            if (txtCode.Password == AdminCode)
            {
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Неверный код.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                txtCode.Clear();
                txtCode.Focus();
            }
        }
    }
}
