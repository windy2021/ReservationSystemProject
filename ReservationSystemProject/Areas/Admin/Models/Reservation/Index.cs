using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace ReservationSystemProject.Areas.Admin.Models.Reservation
{
    public class Index
    {
        public IEnumerable<Data.Reservation> Reservations { get; set; }

        public DateTime Date { get; set; }
        public int SittingId { get; set; }
        public int ReservationStatusId { get; set; }
        public bool IsCapacity { get; set; }

        public SelectList Dates { get; set; }
        public SelectList Sittings { get; set; }
        public SelectList ReservationStatuses { get; set; }
    }
}
