using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystemProject.Models.Reservation
{
    public class Success
    {
        [Display(Name = "Name:")]
        public string FullName { get; set; }

        [Display(Name = "Contact number:")]
        public string MobileNumber { get; set; }

        [Display(Name = "Email:")]
        public string Email { get; set; }

        [Display(Name = "Booked for:")]
        public string SittingName { get; set; }

        [Display(Name = "We will see you on:")]
        public DateTime Date { get; set; }

        [Display(Name = "Duration")]
        public int Duration { get; set; }

        [Display(Name = "Number of guests:")]
        public int Guests { get; set; }

        [Display(Name = "Additional notes:")]
        public string Notes { get; set; }
        public int ReservationId { get; set; }
        public string Message { get; set; }

    }
}
