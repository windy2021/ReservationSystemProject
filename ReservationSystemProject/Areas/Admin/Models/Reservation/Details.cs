using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystemProject.Areas.Admin.Models.Reservation
{
    public class Details
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public int Guests { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get { return Start.AddMinutes(Duration); } }
        public int Duration { get; set; }
        public string Note { get; set; }
        public string ReservationType { get; set; }
        public string ReservationStatus { get; set; }
        public string Sitting { get; set; }
    }
}
