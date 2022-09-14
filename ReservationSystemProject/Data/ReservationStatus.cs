using System.Collections.Generic;

namespace ReservationSystemProject.Data
{
    public class ReservationStatus
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<Reservation> Reservations { get; set; }
    }
}