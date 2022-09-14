using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystemProject.Areas.Member.Models
{
    public class Index
    {
        public IEnumerable<Data.Reservation> Reservations { get; set; }
        public string Type { get; set; }

        public List<string> FilterType { get; set; }
    }
}
