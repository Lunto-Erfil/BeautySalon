using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySalon.Models
{
    public class Appointment
    {
        public string ServiceTitle { get; set; }
        public string ClientFullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime StartTime { get; set; }

        public string RemainingTime
        {
            get
            {
                var left = StartTime - DateTime.Now;
                if (left.TotalMinutes < 0)
                    return "просрочено";

                return $"{(int)left.TotalHours} ч {left.Minutes} мин";
            }
        }

        public bool IsUrgent
        {
            get
            {
                var left = StartTime - DateTime.Now;
                return left.TotalMinutes >= 0 && left.TotalHours < 1;
            }
        }
    }
}
