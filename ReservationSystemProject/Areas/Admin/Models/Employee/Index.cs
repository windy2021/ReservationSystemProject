using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystemProject.Areas.Admin.Models.Employee
{
    public class Index
    {
        public IEnumerable<Data.Person> People { get; set; }
    }
}
