using System;
using System.Collections.Generic;

namespace ReservationSystemProject.Data
{
    public class Reservation
    {
        public int Id { get; set; }
        public int Guests { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get { return Start.AddMinutes(Duration); } }
        public int Duration { get; set; }
        public string Note { get; set; }

        public List<TableReservation> TableReservations { get; set; } = new List<TableReservation>();
        public int PersonId { get; set; }
        public Person Person { get; set; }

        public int ReservationTypeId { get; set; }
        public ReservationType ReservationType { get; set; }

        public int ReservationStatusId { get; set; }
        public ReservationStatus ReservationStatus { get; set; }

        public int SittingId { get; set; }
        public Sitting Sitting { get; set; }
    }
}