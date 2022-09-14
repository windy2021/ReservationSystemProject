using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using ReservationSystemProject.Data;
using ReservationSystemProject.Service;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystemProject.Controllers
{
    public class ReservationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly PersonService _personService;
        private readonly ILogger<HomeController> _logger;
        public ReservationController(ApplicationDbContext context, UserManager<IdentityUser> userManager, PersonService personService, ILogger<HomeController> logger)
        {
            _context = context;
            _userManager = userManager;
            _personService = personService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Create()
        {
            List<DateTime> availableDates = new List<DateTime>();
            var sittings = _context.Sittings;
            foreach (var s in sittings)
            {
                if (s.IsClosed == false && s.IsPrivate == false)
                {
                    var d = s.Start;
                    while (d <= s.End)
                    {
                        availableDates.Add(d);
                        d = d.AddDays(1);
                    }
                }
            }

            ViewData["Dates"] = availableDates;

            var model = new Models.Reservation.Create
            {
                Guests = 1,
            };

            if (User.IsInRole("Member"))
            {
                var user = _context.People.FirstOrDefault(p => p.Email == User.Identity.Name);
                var member = new Models.Reservation.Create
                {
                    Email = user.Email,
                    FullName = user.FullName,
                    MobileNumber = user.MobileNumber,
                    Guests = 1,
                };
                return View(member);
            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Create(Models.Reservation.Create model)
        {
            Person person = null;
            person = _context.People.FirstOrDefault(p => p.Email == model.Email);
            if (ModelState.IsValid)
            {
                try
                {
                    DateTime reservationDate = DateTime.Parse($"{model.Date.ToShortDateString()} {model.Time}");
                    var sittings = _context.Sittings;
                    int reservationSittingId = 0;
                    foreach (var sitting in sittings)
                    {
                        if (DateTime.Compare(reservationDate, sitting.Start) == 0 || DateTime.Compare(reservationDate, sitting.End) == 0)
                        {
                            reservationSittingId = sitting.Id;
                            break;
                        }
                        else if (DateTime.Compare(reservationDate, sitting.Start) > 0 && DateTime.Compare(reservationDate, sitting.End) < 0)
                        {
                            reservationSittingId = sitting.Id;
                            break;
                        }
                    }
                    if (person == null)
                    {
                        person = new Person
                        {
                            FullName = model.FullName,
                            Email = model.Email,
                            MobileNumber = model.MobileNumber
                        };
                        person = await _personService.UpsertPersonAsync(person, false);

                        var reservation = new Reservation
                        {
                            Person = person,
                            Guests = model.Guests,
                            Start = reservationDate,
                            Duration = model.Duration,
                            Note = model.Notes,
                            SittingId = reservationSittingId,
                            ReservationTypeId = 1,
                            ReservationStatusId = 1
                        };
                        _context.Reservations.Add(reservation);
                        await _context.SaveChangesAsync();
                        TempData["reservationId"] = reservation.Id.ToString();
                        TempData["Message"] = "Thank you for your reservation, we'll be in contact with you shortly! :)";
                    }
                    else
                    {
                        var reservation = new Reservation
                        {
                            Person = person,
                            Guests = model.Guests,
                            Start = reservationDate,
                            Duration = model.Duration,
                            Note = model.Notes,
                            SittingId = reservationSittingId,
                            ReservationTypeId = 1,
                            ReservationStatusId = 1
                        };
                        _context.Reservations.Add(reservation);
                        await _context.SaveChangesAsync();
                        TempData["reservationId"] = reservation.Id.ToString();
                        TempData["Message"] = "Thank you! Looks like you have an account with us, login to change your details.";
                        return RedirectToAction("Success");
                    }
                }
                catch (Exception e)
                {
                    _logger.LogWarning("Failed to create Reservation. Exception message: " + e);
                }
            }
            else
            {
                List<DateTime> availableDates = new List<DateTime>();
                var sittings = _context.Sittings;
                foreach (var s in sittings)
                {
                    if (s.IsClosed == false && s.IsPrivate == false)
                    {
                        var d = s.Start;
                        while (d <= s.End)
                        {
                            availableDates.Add(d);
                            d = d.AddDays(1);
                        }
                    }
                }

                ViewData["Dates"] = availableDates;
                return View(model);
            }
            return RedirectToAction("Success");
        }
        public IActionResult Success(Models.Reservation.Success model)
        {
            if (TempData.ContainsKey("reservationId"))
            {
                Int32.TryParse(TempData["reservationId"].ToString(), out int reservationId);
                var reservation = _context.Reservations.FirstOrDefault(r => r.Id == reservationId);
                var person = _context.People.FirstOrDefault(p => p.Id == reservation.PersonId);
                model.FullName = person.FullName;
                model.Email = person.Email;
                model.MobileNumber = person.MobileNumber;
                model.Date = reservation.Start;
                model.Guests = reservation.Guests;
                model.Notes = reservation.Note;
                model.Duration = reservation.Duration;
                model.SittingName = _context.Sittings.FirstOrDefault(s => s.Id == reservation.SittingId).Name;
                model.Message = TempData["Message"].ToString();
            }
            return View(model);
        }
    }
}
