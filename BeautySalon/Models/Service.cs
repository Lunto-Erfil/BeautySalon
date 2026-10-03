using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BeautySalon.Models
{
    public class Service : INotifyPropertyChanged
    {
        private int _id;
        private string _title;
        private decimal _cost;
        private int _durationInSeconds;
        private string _description;
        private double? _discount;
        private string _mainImagePath;

        public int ID
        {
            get => _id;
            set { _id = value; OnPropertyChanged(); }
        }

        public string Title
        {
            get => _title;
            set { _title = value; OnPropertyChanged(); }
        }

        public decimal Cost
        {
            get => _cost;
            set
            {
                _cost = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FinalCost));
            }
        }

        public int DurationInSeconds
        {
            get => _durationInSeconds;
            set
            {
                _durationInSeconds = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DurationMinutes));
            }
        }

        public string Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(); }
        }

        public double? Discount
        {
            get => _discount;
            set
            {
                _discount = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FinalCost));
                OnPropertyChanged(nameof(HasDiscount));
            }
        }

        public string MainImagePath
        {
            get => _mainImagePath;
            set { _mainImagePath = value; OnPropertyChanged(); }
        }

        public int DurationMinutes => DurationInSeconds / 60;

        public bool HasDiscount => Discount.HasValue && Discount.Value > 0;

        public decimal FinalCost => HasDiscount
            ? Cost * (decimal)(1 - Discount.Value / 100.0)
            : Cost;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
