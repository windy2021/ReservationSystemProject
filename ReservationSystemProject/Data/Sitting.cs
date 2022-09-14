using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystemProject.Data
{
    public class Sitting
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public int Capacity { get; set; }
        
        public int Pax { get { return Reservations.Sum(r => r.Guests); } }
        public int Vacancies { get { return Capacity - Pax; } }
        
        public bool IsPrivate { get; set; }
        public bool IsClosed { get; set; }

        public List<Reservation> Reservations { get; set; } = new List<Reservation>();

        public int RestaurantId { get; set; }
        public Restaurant Restaurant { get; set; }
    }
}
