using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystemProject.Areas.Admin.Models.Sitting
{
    public class Create
    {
        [Display(Name = "Name of Sitting")]
        [Required(ErrorMessage = "Please enter a name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please select a date")]
        public DateTime Date { get; set; }

        [Display(Name = "Start Time")]
        [Range(typeof(TimeSpan), "08:00", "19:00", ErrorMessage = "Start Time must be between 8am and 7pm")]
        public TimeSpan Start { get; set; }

        [Display(Name = "End Time")]
        [Range(typeof(TimeSpan), "9:00", "23:00", ErrorMessage = "End Time must be between 9am and 11pm")]
        public TimeSpan End { get; set; }

        [Required(ErrorMessage = "Please enter capacity")]
        [Range(10, 1000, ErrorMessage = "Minimum capacity is 10")]
        public int Capacity { get; set; }

        [Display(Name = "Is Sitting Private?")]
        public bool IsPrivate { get; set; }

        [Display(Name = "Is Sitting Closed?")]
        public bool IsClosed { get; set; }
    }
}
