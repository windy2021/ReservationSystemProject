using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystemProject.Areas.Admin.Models.Sitting
{
    public class CreateTable
    {
        [Display(Name = "Table Name")]
        [Required(ErrorMessage = "Name must be entered")]
        public string Name { get; set; }
        public int AreaId { get; set; }
    }
}
