using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class ElectricityPrice
    {
        public decimal SEK_per_kWh { get; set; }
        public decimal EUR_per_kWh { get; set; }
        public decimal EXR { get; set; }
        public DateTime time_start { get; set; }
        public DateTime time_end { get; set; }
    }
}
