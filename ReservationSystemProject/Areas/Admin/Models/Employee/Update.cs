using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystemProject.Areas.Admin.Models.Employee
{
    public class Update
    {
        public int Id { get; set; }

        [Display(Name = "Name")]
        [Required(ErrorMessage = "Please enter name.")]
        [StringLength(50, ErrorMessage = "Name is too long")]
        public string FullName { get; set; }

        public string Email { get; set; }
        [Display(Name = "Mobile number")]
        [Required(ErrorMessage = "Please enter contact number.")]
        [StringLength(12, ErrorMessage = "Not a valid phone number")]
        public string MobileNumber { get; set; }

        public string Role { get; set; }
        public SelectList Roles { get; set; }
    }
}
