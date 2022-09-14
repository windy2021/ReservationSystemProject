using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservationSystemProject.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystemProject.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReservationController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet,Route("{id?}")]
        public async Task<IActionResult> Get(int? id)
        {
            if (id.HasValue)
            {
                var reservation = await _context.Reservations.FirstOrDefaultAsync(r => r.Id == id.Value);
                if (reservation != null) return Ok(reservation);
                return NotFound();
            }
            var reservations = await _context.Reservations.ToListAsync();
            return Ok(reservations);
        }

        [HttpGet,Route("user/{email?}")]
        public async Task<IActionResult> Get(string email)
        {
            if(email != null)
            {
                var reservations = await _context.Reservations.Include(r => r.Person).Where(r => r.Person.Email == email).ToListAsync();
                if (reservations != null) return Ok(reservations);
                return NotFound();
            }
            return NotFound();
        }

        [HttpGet, Route("gettimes/{selectedDate}")]
        public IActionResult GetTimes(string selectedDate)
        {
            if (selectedDate != null)
            {
                List<string> timeList = new List<string>();

                DateTime datetime = DateTime.Parse(selectedDate);

                string start;
                string end;

                var availableSittings = _context.Sittings;

                foreach (var sitting in availableSittings)
                {
                    if (DateTime.Compare(datetime.Date, sitting.Start.Date) == 0 || DateTime.Compare(datetime.Date, sitting.End.Date) == 0)
                    {
                        start = sitting.Start.ToString("HH:mm");
                        end = sitting.End.ToString("HH:mm");
                        DateTime startT = DateTime.Parse($"{selectedDate} {start}");
                        DateTime endT = DateTime.Parse($"{selectedDate} {end}");
                        endT = endT.AddMinutes(-60);
                        while (DateTime.Compare(startT, endT) <= 0)
                        {
                            timeList.Add(startT.ToString("HH:mm"));
                            startT = startT.AddMinutes(15);
                        }
                    }
                    else if (DateTime.Compare(datetime.Date, sitting.Start.Date) > 0 && DateTime.Compare(datetime.Date, sitting.End.Date) < 0)
                    {
                        start = sitting.Start.ToString("HH:mm");
                        end = sitting.End.ToString("HH:mm");
                        DateTime startT = DateTime.Parse($"{selectedDate} {start}");
                        DateTime endT = DateTime.Parse($"{selectedDate} {end}");
                        endT = endT.AddMinutes(-60);
                        while (DateTime.Compare(startT, endT) <= 0)
                        {
                            timeList.Add(startT.ToString("HH:mm"));
                            startT = startT.AddMinutes(15);
                        }
                    }

                }
                return Ok(timeList);
            }
            return NotFound();
        }
    }
}
