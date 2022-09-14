using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystemProject.Areas.Admin.Models.Sitting
{
    public class Index
    {
        public IEnumerable<Data.Sitting> Sittings { get; set; }
        public IEnumerable<Data.Area> Areas { get; set; }
        public DateTime Date { get; set; }
        public SelectList Dates { get; set; }
    }
}
