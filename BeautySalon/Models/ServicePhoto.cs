using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySalon.Models
{
    public class ServicePhoto
    {
        public int ID { get; set; }
        public int ServiceID { get; set; }
        public string PhotoPath { get; set; }
    }
}
