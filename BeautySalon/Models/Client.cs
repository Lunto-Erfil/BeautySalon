using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySalon.Models
{ public class Client
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string FullName
        {
            get
            {
                var s = (LastName ?? string.Empty) + " "
                      + (FirstName ?? string.Empty) + " "
                      + (Patronymic ?? string.Empty);
                return s.Trim();
            }
        }
    }
}
