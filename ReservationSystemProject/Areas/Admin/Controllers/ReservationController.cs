using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ReservationSystemProject.Data;
using ReservationSystemProject.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystemProject.Areas.Admin.Controllers
{
    [Area("Admin"),Authorize(Roles = "Manager,Staff,Admin")]
    public class ReservationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PersonService _personService;
        public ReservationController(ApplicationDbContext context, PersonService personService) 
        {
            _context = context;
            _personService = personService;
        }
        [HttpGet]
        public IActionResult Index()
        {
            var m = new Models.Reservation.Index();
            //add reservations to model
            m.Reservations = _context.Reservations
                .Include(r => r.Sitting)
                .Include(r => r.ReservationStatus)
                .Include(r => r.Person);
            //filter dates and add them to list
            m.Dates = new SelectList(getDates());
            return View(m);
        }

        [HttpPost]
        public IActionResult Index(Models.Reservation.Index m)
        {
            if(m.Date.Ticks == 0) return RedirectToAction("");
            m.Sittings = new SelectList(_context.Sittings.Where(s => s.Start.Date == m.Date.Date), nameof(Sitting.Id), nameof(Sitting.Name), m.SittingId);
            m.Reservations = _context.Reservations
                .Include(r => r.Sitting)
                .Include(r => r.ReservationStatus)
                .Include(r => r.Person)
                .Where(r => r.Start.Date == m.Date.Date);
            if (m.SittingId != 0) m.Reservations = m.Reservations.Where(r => r.SittingId == m.SittingId);
            m.ReservationStatuses = new SelectList(getStatuses(m.Reservations), nameof(ReservationStatus.Id), nameof(ReservationStatus.Name), m.ReservationStatusId);
            if (m.ReservationStatusId != 0) m.Reservations = m.Reservations.Where(r => r.ReservationStatusId == m.ReservationStatusId);
            m.Dates = new SelectList(getDates(),m.Date.Date);
            return View(m);
        }

        private IEnumerable<ReservationStatus> getStatuses(IEnumerable<Reservation> reservations)
        {
            var statuses = new List<ReservationStatus>();
            foreach (var r in reservations)
            {
                if (!statuses.Contains(r.ReservationStatus))
                {
                    statuses.Add(r.ReservationStatus);
                }
            }
            return statuses;
        }

        private IEnumerable<DateTime> getDates()
        {
            DateTime temp = _context.Sittings.First().Start.Date;
            List<DateTime> dates = new List<DateTime>() { temp };
            foreach (var s in _context.Sittings)
            {
                if (temp.CompareTo(s.Start.Date) < 0)
                {
                    dates.Add(s.Start.Date);
                    temp = s.Start.Date;
                }
            }
            return dates;
        }

        [HttpGet]
        public IActionResult Create()
        {
            var m = new Models.Reservation.Create();
            m.ReservationStatuses = new SelectList(_context.ReservationStatuses,nameof(ReservationStatus.Id),nameof(ReservationStatus.Name));
            m.ReservationTypes = new SelectList(_context.ReservationTypes, nameof(ReservationType.Id), nameof(ReservationType.Name));
            var tables = _context.Tables.Select(t => new {
                t.Id,
                t.Name,
                Occupied = _context.TableReservations.FirstOrDefault(tr => tr.TableId == t.Id) != null ? "Occupied" : "Unoccupied" 
            });
            m.Tables = new SelectList(tables, nameof(Table.Id), nameof(Table.Name), null, "Occupied");
            return View(m);
        }

        [HttpPost]
        public IActionResult Create(Models.Reservation.Create m)
        {
            if (ModelState.IsValid)
            {
                Person person = null;
                if(m.Email != null)
                {
                    var task = _personService.UpsertPersonAsync(new Person
                    {
                        FullName = m.Name,
                        Email = m.Email,
                        MobileNumber = m.Mobile
                    }, false);
                    task.Wait();
                    person = task.Result;
                } else
                {
                    person = _context.People.Add(new Person
                    {
                        FullName = m.Name,
                        Email = "annon",
                        MobileNumber = m.Mobile != null ? m.Mobile : "annon"
                    }).Entity;
                    _context.SaveChanges();
                }
                var date = DateTime.Parse(m.Date + " " + m.Start);
                var res = _context.Reservations.Add(new Reservation()
                {
                    Start = date,
                    Duration = m.Duration,
                    Guests = m.Guests,
                    ReservationStatusId = m.ReservationStatusId,
                    ReservationTypeId = m.ReservationTypeId,
                    Note = m.Note,
                    SittingId = _context.Sittings.FirstOrDefault(s => s.Start.CompareTo(date) <= 0 && s.End.CompareTo(date) >= 0).Id,
                    PersonId = person.Id
                });
                _context.SaveChanges();
                if (m.TableId != 0)
                {
                    _context.TableReservations.Add(new TableReservation { ReservationId = res.Entity.Id, TableId = m.TableId });
                }
                _context.SaveChanges();
                return RedirectToAction("");
            }
            m.ReservationStatuses = new SelectList(_context.ReservationStatuses, nameof(ReservationStatus.Id), nameof(ReservationStatus.Name));
            m.ReservationTypes = new SelectList(_context.ReservationTypes, nameof(ReservationType.Id), nameof(ReservationType.Name));
            var tables = _context.Tables.Select(t => new {
                t.Id,
                t.Name,
                Occupied = _context.TableReservations.FirstOrDefault(tr => tr.TableId == t.Id) != null ? "Occupied" : "Unoccupied"
            });
            m.Tables = new SelectList(tables, nameof(Table.Id), nameof(Table.Name), null, "Occupied");
            return View(m);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var res = _context.Reservations.Include(r => r.Person).FirstOrDefault(r => r.Id == id);
            if (res == null) return NotFound();
            var m = new Models.Reservation.Edit()
            {
                Id = id,
                Name = res.Person.FullName,
                Mobile = res.Person.MobileNumber,
                Email = res.Person.Email,
                Guests = res.Guests,
                Date = res.Start.Date.ToString("yyyy-MM-dd"),
                Start = res.Start.ToString("HH:mm tt"),
                Duration = res.Duration,
                ReservationTypeId = res.ReservationTypeId,
                ReservationStatusId = res.ReservationStatusId,
                TableId = _context.TableReservations.FirstOrDefault(tr => tr.ReservationId == id).TableId,
                ReservationTypes = new SelectList(_context.ReservationTypes, nameof(ReservationType.Id), nameof(ReservationType.Name)),
                ReservationStatuses = new SelectList(_context.ReservationStatuses, nameof(ReservationStatus.Id), nameof(ReservationStatus.Name))
            };
            var tables = _context.Tables.Select(t => new {
                t.Id,
                t.Name,
                Occupied = _context.TableReservations.FirstOrDefault(tr => tr.TableId == t.Id) != null ? "Occupied" : "Unoccupied"
            });
            m.Tables = new SelectList(tables, nameof(Table.Id), nameof(Table.Name), null, "Occupied");
            return View(m);
        }
        [HttpPost]
        public IActionResult Edit(Models.Reservation.Edit m)
        {
            if (ModelState.IsValid)
            {
                var res = _context.Reservations.Include(r => r.Person).FirstOrDefault(r => r.Id == m.Id);
                var date = DateTime.Parse(m.Date + " " + m.Start);
                res.Guests = m.Guests;
                res.Start = date;
                res.Duration = m.Duration;
                res.ReservationTypeId = m.ReservationTypeId;
                res.ReservationStatusId = m.ReservationStatusId;
                var table = _context.TableReservations.FirstOrDefault(tr => tr.ReservationId == m.Id);
                if(table != null)
                {
                    table.TableId = m.TableId;
                } else
                {
                    _context.TableReservations.Add(new TableReservation { ReservationId = m.Id, TableId = m.TableId });
                }
                _context.SaveChanges();
                return RedirectToAction("");
            }
            var tables = _context.Tables.Select(t => new {
                t.Id,
                t.Name,
                Occupied = _context.TableReservations.FirstOrDefault(tr => tr.TableId == t.Id) != null ? "Occupied" : "Unoccupied"
            });
            m.Tables = new SelectList(tables, nameof(Table.Id), nameof(Table.Name), null, "Occupied");
            return View(m);
        }
        [HttpGet]
        public IActionResult Details(int id)
        {
            var res = _context.Reservations
                .Include(r => r.Person)
                .Include(r => r.ReservationStatus)
                .Include(r => r.ReservationType)
                .Include(r => r.Sitting)
                .FirstOrDefault(r => r.Id == id);
            if(res == null) return NotFound();
            var m = new Models.Reservation.Details()
            {
                Id = res.Id,
                Name = res.Person.FullName,
                Email = res.Person.Email,
                Mobile = res.Person.MobileNumber,
                Guests = res.Guests,
                Start = res.Start,
                Duration = res.Duration,
                ReservationStatus = res.ReservationStatus.Name,
                ReservationType = res.ReservationType.Name,
                Sitting = res.Sitting.Name
            };
            return View(m);
        }
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var res = _context.Reservations
                .Include(r => r.Person)
                .Include(r => r.ReservationStatus)
                .Include(r => r.ReservationType)
                .Include(r => r.Sitting)
                .FirstOrDefault(r => r.Id == id);
            if (res == null) return NotFound();
            var m = new Models.Reservation.Details()
            {
                Id = res.Id,
                Name = res.Person.FullName,
                Email = res.Person.Email,
                Mobile = res.Person.MobileNumber,
                Guests = res.Guests,
                Start = res.Start,
                Duration = res.Duration,
                ReservationStatus = res.ReservationStatus.Name,
                ReservationType = res.ReservationType.Name,
                Sitting = res.Sitting.Name
            };
            return View(m);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var tableres = _context.TableReservations.Where(tr => tr.ReservationId == id);
            foreach (var t in tableres)
            {
                _context.Remove(t);
            }
            _context.Remove(_context.Reservations.FirstOrDefault(r => r.Id == id));
            _context.SaveChanges();
            return RedirectToAction("");
        }
    }
}
