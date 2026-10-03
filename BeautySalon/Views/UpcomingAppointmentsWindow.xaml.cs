using BeautySalon.Data;
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
using System.Windows.Threading;

namespace BeautySalon.Views
{
    public partial class UpcomingAppointmentsWindow : Window
    {
        private readonly AppointmentRepository _repo = new AppointmentRepository();
        private readonly DispatcherTimer _timer = new DispatcherTimer();

        public UpcomingAppointmentsWindow()
        {
            InitializeComponent();

            _timer.Interval = TimeSpan.FromSeconds(30);
            _timer.Tick += (s, e) => Load();
            _timer.Start();

            Load();
        }

        protected override void OnClosed(EventArgs e)
        {
            _timer.Stop();
            base.OnClosed(e);
        }

        private void Load()
        {
            dgAppointments.ItemsSource = _repo.GetUpcoming().DefaultView;
            lblUpdated.Text = $"Обновлено: {DateTime.Now:HH:mm:ss}";
        }
    }
}
