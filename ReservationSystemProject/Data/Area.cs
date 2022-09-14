using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystemProject.Data
{
    public class Area
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<Table> Tables { get; set; }

        public int RestaurantId { get; set; }
        public Restaurant Restaurant { get; set; }
    }
}
