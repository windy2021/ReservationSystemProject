using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystemProject.Models.Reservation
{
    public class Create
    {
        [Display(Name = "Name")]
        [Required(ErrorMessage = "Please enter your name")]
        [StringLength(50, ErrorMessage = "Name is too long")]
        public string FullName { get; set; }

        [Display(Name = "Number of Guests")]
        [Range(1, 10, ErrorMessage = "Call us to make a booking for more than 10 people")]
        public int Guests { get; set; }

        [Required(ErrorMessage = "Please enter your email")]
        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "Email is not valid.")]
        public string Email { get; set; }

        [Display(Name = "Mobile Number")]
        [Required(ErrorMessage = "Please enter your mobile number")]
        [StringLength(12, ErrorMessage = "Not a valid phone number")]
        public string MobileNumber { get; set; }

        [Range(45, 120, ErrorMessage = "Duration should be between 45 - 120 minutes")]
        public int Duration { get; set; }

        [Display(Name = "Additional Notes")]
        public string Notes { get; set; }

        [Display(Name = "Booking Date")]
        [Required(ErrorMessage = "Please pick a date")]
        [DataType(DataType.Date, ErrorMessage ="Please pick a date")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Pick a date first.")]
        public string Time { get; set; }
        //public SelectList Dates { get; set; }
        //public SelectList Times { get; set; }
        //public int PersonId { get; set; }
        //public int SittingId { get; set; }
        //public SelectList Sittings { get; set; }
    }
}
