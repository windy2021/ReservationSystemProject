using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystemProject.Data
{
    public class Restaurant
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public int ABN { get; set; }

        public List<Sitting> Sittings { get; set; } = new List<Sitting>();

        public List<Area> Areas { get; set; } = new List<Area>();
    }
}
