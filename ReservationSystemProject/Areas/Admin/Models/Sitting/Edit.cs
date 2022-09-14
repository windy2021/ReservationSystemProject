using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystemProject.Areas.Admin.Models.Sitting
{
    public class Edit : Sitting.Create
    {
        [Required]
        public int Id { get; set; }
    }
}
