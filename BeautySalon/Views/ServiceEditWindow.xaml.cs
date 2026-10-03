using BeautySalon.Data;
using BeautySalon.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
    public partial class ServiceEditWindow : Window
    {
        private readonly ServiceRepository _repo = new ServiceRepository();
        private readonly Service _service;
        private readonly ObservableCollection<ServicePhoto> _photos = new ObservableCollection<ServicePhoto>();
        private string _mainImagePath;

        public ServiceEditWindow(Service service)
        {
            InitializeComponent();
            _service = service;
            lstPhotos.ItemsSource = _photos;

            if (_service == null)
            {
                lblHeader.Text = "Добавление услуги";
                Title = "Добавление услуги";
            }
            else
            {
                lblHeader.Text = "Редактирование услуги";
                Title = "Редактирование услуги";

                txtTitle.Text = _service.Title;
                txtCost.Text = _service.Cost.ToString("0.##");
                txtDuration.Text = _service.DurationMinutes.ToString();
                txtDiscount.Text = _service.Discount?.ToString("0.##") ?? string.Empty;
                txtDescription.Text = _service.Description;

                _mainImagePath = _service.MainImagePath;
                ShowMainImage();

                foreach (var p in _repo.GetPhotos(_service.ID))
                    _photos.Add(p);
            }
        }

        private void ShowMainImage()
        {
            if (string.IsNullOrWhiteSpace(_mainImagePath))
            {
                imgMain.Source = null;
                return;
            }

            var full = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, _mainImagePath);
            if (!File.Exists(full)) return;

            var bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.CacheOption = BitmapCacheOption.OnLoad;
            bmp.UriSource = new System.Uri(full);
            bmp.EndInit();
            imgMain.Source = bmp;
        }

        private void BtnChooseMain_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                Filter = "Изображения|*.jpg;*.jpeg;*.png;*.bmp"
            };

            if (ofd.ShowDialog() != true) return;

            _mainImagePath = ServiceRepository.CopyImageToProject(ofd.FileName);
            ShowMainImage();
        }

        private void BtnAddPhoto_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                Filter = "Изображения|*.jpg;*.jpeg;*.png;*.bmp",
                Multiselect = true
            };

            if (ofd.ShowDialog() != true) return;

            foreach (var file in ofd.FileNames)
            {
                var rel = ServiceRepository.CopyImageToProject(file);

                if (_service == null)
                {
                    _photos.Add(new ServicePhoto { ID = 0, PhotoPath = rel });
                }
                else
                {
                    var id = _repo.AddPhoto(_service.ID, rel);
                    _photos.Add(new ServicePhoto { ID = id, ServiceID = _service.ID, PhotoPath = rel });
                }
            }
        }

        private void BtnRemovePhoto_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is FrameworkElement fe) || !(fe.Tag is ServicePhoto photo)) return;

            if (photo.ID > 0) _repo.DeletePhoto(photo.ID);
            _photos.Remove(photo);
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            var title = txtTitle.Text.Trim();

            if (string.IsNullOrWhiteSpace(title))
            {
                MessageBox.Show("Введите название услуги.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!decimal.TryParse(txtCost.Text, out var cost) || cost < 0)
            {
                MessageBox.Show("Некорректная стоимость.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!int.TryParse(txtDuration.Text, out var minutes) || minutes <= 0 || minutes > 240)
            {
                MessageBox.Show("Длительность должна быть от 1 до 240 минут.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            double? discount = null;
            if (!string.IsNullOrWhiteSpace(txtDiscount.Text))
            {
                if (!double.TryParse(txtDiscount.Text, out var d) || d < 0 || d > 100)
                {
                    MessageBox.Show("Скидка должна быть от 0 до 100%.", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                discount = d;
            }

            var currentId = _service?.ID ?? 0;

            if (_repo.TitleExists(title, currentId))
            {
                MessageBox.Show("Услуга с таким названием уже существует.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var service = new Service
            {
                ID = currentId,
                Title = title,
                Cost = cost,
                DurationInSeconds = minutes * 60,
                Description = txtDescription.Text.Trim(),
                Discount = discount,
                MainImagePath = _mainImagePath
            };

            if (_service == null)
            {
                var newId = _repo.Insert(service);

                foreach (var p in _photos)
                {
                    if (p.ID == 0)
                    {
                        p.ID = _repo.AddPhoto(newId, p.PhotoPath);
                        p.ServiceID = newId;
                    }
                }
            }
            else
            {
                _repo.Update(service);
            }

            DialogResult = true;
            Close();
        }
    }
}