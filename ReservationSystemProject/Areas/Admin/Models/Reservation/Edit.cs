using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystemProject.Areas.Admin.Models.Reservation
{
    public class Edit
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Please enter a name")]
        public string Name { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        [Required(ErrorMessage="Please enter number of guests"),Range(1,100,ErrorMessage="Please enter at least 1 guest")]
        public int Guests { get; set; }
        [Required(ErrorMessage="Please select a date")]
        public string Date { get; set; }
        [Required(ErrorMessage="Please select a time")]
        public string Start { get; set; }
        [Required(ErrorMessage="Please enter a duration"),Range(15,120,ErrorMessage="Duration must be 15 min or longer")]
        public int Duration { get; set; }
        public string Note { get; set; }
        public int TableId { get; set; }
        public SelectList Tables { get; set; }
        [Required(ErrorMessage = "Please select a reservation type"),Range(1,int.MaxValue,ErrorMessage = "Please select a reservation type")]
        public int ReservationTypeId { get; set; }
        public SelectList ReservationTypes { get; set; }
        [Required(ErrorMessage = "Please select a reservation status"), Range(1, int.MaxValue, ErrorMessage = "Please select a reservation type")]
        public int ReservationStatusId { get; set; }
        public SelectList ReservationStatuses { get; set; }
    }
}
