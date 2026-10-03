using BeautySalon.Data;
using BeautySalon.Models;
using BeautySalon.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace BeautySalon
{
        public partial class MainWindow : Window
        {
            private readonly ServiceRepository _serviceRepo = new ServiceRepository();
            private readonly ObservableCollection<Service> _services = new ObservableCollection<Service>();
            private bool _isAdmin;

            public MainWindow()
            {
                InitializeComponent();
                dgServices.ItemsSource = _services;
                cmbDiscount.SelectedIndex = 0;
                Loaded += (s, e) => LoadServices();
            }

            private void LoadServices()
            {
                var (min, max) = GetDiscountRange();
                bool asc = rbAsc.IsChecked == true;

                var list = _serviceRepo.Search(txtSearch.Text.Trim(), min, max, asc);

                _services.Clear();
                foreach (var s in list) _services.Add(s);

                lblCount.Text = $"{_services.Count} из {_serviceRepo.GetTotalCount()}";
            }

            private (double? min, double? max) GetDiscountRange()
            {
                switch (cmbDiscount.SelectedIndex)
                {
                    case 1: return (0, 5);
                    case 2: return (5, 15);
                    case 3: return (15, 30);
                    case 4: return (30, 70);
                    case 5: return (70, 100);
                    default: return (null, null);
                }
            }

            private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
            {
                if (IsLoaded) LoadServices();
            }

            private void CmbDiscount_SelectionChanged(object sender, SelectionChangedEventArgs e)
            {
                if (IsLoaded) LoadServices();
            }

            private void RbSort_Checked(object sender, RoutedEventArgs e)
            {
                if (IsLoaded) LoadServices();
            }

            private void BtnAdmin_Click(object sender, RoutedEventArgs e)
            {
                if (_isAdmin)
                {
                    pnlAdmin.Visibility = Visibility.Collapsed;
                    _isAdmin = false;
                    return;
                }

                var dlg = new AdminLoginWindow { Owner = this };
                if (dlg.ShowDialog() == true)
                {
                    _isAdmin = true;
                    pnlAdmin.Visibility = Visibility.Visible;
                }
            }

            private void BtnAdd_Click(object sender, RoutedEventArgs e)
            {
                var dlg = new ServiceEditWindow(null) { Owner = this };
                if (dlg.ShowDialog() == true) LoadServices();
            }

            private void BtnEdit_Click(object sender, RoutedEventArgs e)
            {
                if (dgServices.SelectedItem is Service s)
                {
                    var dlg = new ServiceEditWindow(s) { Owner = this };
                    if (dlg.ShowDialog() == true) LoadServices();
                }
            }

            private void BtnDelete_Click(object sender, RoutedEventArgs e)
            {
                if (!(dgServices.SelectedItem is Service s)) return;

                if (_serviceRepo.HasAppointments(s.ID))
                {
                    MessageBox.Show(
                        "Нельзя удалить услугу: по ней есть записи клиентов.",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (MessageBox.Show(
                        "Удалить услугу?", "Подтверждение",
                        MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    _serviceRepo.Delete(s.ID);
                    LoadServices();
                }
            }

            private void BtnAppointment_Click(object sender, RoutedEventArgs e)
            {
                if (dgServices.SelectedItem is Service s)
                {
                    var dlg = new AppointmentWindow(s) { Owner = this };
                    dlg.ShowDialog();
                }
            }

            private void BtnUpcoming_Click(object sender, RoutedEventArgs e)
            {
                var dlg = new UpcomingAppointmentsWindow { Owner = this };
                dlg.ShowDialog();
            }
        }
    }
