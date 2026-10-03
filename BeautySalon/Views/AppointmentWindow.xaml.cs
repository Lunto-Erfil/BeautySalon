using BeautySalon.Data;
using BeautySalon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    public partial class AppointmentWindow : Window
    {
        private readonly ClientRepository _clientRepo = new ClientRepository();
        private readonly AppointmentRepository _apptRepo = new AppointmentRepository();
        private readonly Service _service;

        public AppointmentWindow(Service service)
        {
            InitializeComponent();
            _service = service;

            lblService.Text = "Услуга: " + _service.Title;
            lblDuration.Text = $"Длительность: {_service.DurationMinutes} мин";

            cmbClient.ItemsSource = _clientRepo.GetAll();
            dpDate.SelectedDate = DateTime.Today;
        }

        private void TxtTime_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, @"[0-9:]");
        }

        private void TxtTime_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TryParseTime(txtTime.Text, out var time))
            {
                var end = time.Add(TimeSpan.FromMinutes(_service.DurationMinutes));
                lblEndTime.Text = $"Окончание: {end:hh\\:mm}";
            }
            else
            {
                lblEndTime.Text = "Окончание: --:--";
            }
        }

        private static bool TryParseTime(string text, out TimeSpan time)
        {
            time = default;
            if (string.IsNullOrWhiteSpace(text)) return false;
            if (!Regex.IsMatch(text, @"^\d{1,2}:\d{2}$")) return false;
            return TimeSpan.TryParseExact(text, new[] { @"h\:mm", @"hh\:mm" },
                System.Globalization.CultureInfo.InvariantCulture, out time);
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!(cmbClient.SelectedItem is Client client))
            {
                MessageBox.Show("Выберите клиента.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (dpDate.SelectedDate == null)
            {
                MessageBox.Show("Укажите дату.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!TryParseTime(txtTime.Text, out var time))
            {
                MessageBox.Show("Введите корректное время, например 14:30.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var start = dpDate.SelectedDate.Value.Date.Add(time);
            _apptRepo.Create(client.ID, _service.ID, start);

            MessageBox.Show("Запись сохранена.", "Готово",
                MessageBoxButton.OK, MessageBoxImage.Information);

            DialogResult = true;
            Close();
        }
    }
}
