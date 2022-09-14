using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ReservationSystemProject.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystemProject.Areas.Member.Controllers
{
    [Area("Member"), Authorize(Roles = "Member")]
    public class ReservationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public ReservationController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        [HttpGet]
        public IActionResult Index()
        {
            var person = _context.People.FirstOrDefault(p => p.Email == User.Identity.Name);
            var reservations = _context.Reservations;
            List<Reservation> reservationList = new List<Reservation>();

            foreach (var reservation in reservations)
            {
                if (reservation.PersonId == person.Id)
                {
                    reservationList.Add(reservation);
                }
            }

            var m = new Models.Index();

            m.Reservations = reservationList;
            m.FilterType = getFilterTypes();
            return View(m);
        }

        [HttpPost]
        public IActionResult Index(Models.Index m)
        {
            var person = _context.People.FirstOrDefault(p => p.Email == User.Identity.Name);
            var reservations = _context.Reservations;
            List<Reservation> reservationList = new List<Reservation>();

            foreach (var reservation in reservations)
            {
                if (reservation.PersonId == person.Id)
                {
                    reservationList.Add(reservation);
                }
            }
            if (m.Type == "") return RedirectToAction("");

            if(m.Type == "Upcoming")
            {
                m.Reservations = reservationList
                    .Where(r => r.Start >= DateTime.Now);
            }

            if(m.Type == "Past")
            {
                m.Reservations = reservationList
                    .Where(r => r.Start < DateTime.Now);
            }

            m.FilterType = getFilterTypes();

            return View(m);
        }

        private List<string> getFilterTypes()
        {
            List<string> types = new List<string>
            {
                "Upcoming",
                "Past"
            };

            return types;
        } 
    }
}
