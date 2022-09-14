using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReservationSystemProject.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystemProject.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class SittingController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public SittingController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet, Route("dates")]
        public IActionResult Get()
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
            return Ok(dates);
        }
        [HttpGet, Route("times")]
        public IActionResult Get(string date)
        {
            if(date != null)
            {
                DateTime datetime = DateTime.Parse(date);
                List<Sitting> sittings = _context.Sittings.Where(s => s.Start.Date == datetime.Date).ToList();
                sittings.Sort((a,b) => a.Start.CompareTo(b.Start));
                List<Array> gaps = null;
                if (sittings.Count > 1)
                {
                    gaps = new List<Array>();
                    for (int i = 0; i < sittings.Count - 1; i++)
                    {
                        gaps.Add(new[] { string.Format("{0:t}", sittings[i].End), string.Format("{0:t}", sittings[i + 1].Start) });
                    }
                }
                var data = new {
                    minTime = string.Format("{0:t}", sittings.First().Start),
                    maxTime = string.Format("{0:t}", sittings.Last().End),
                    disableTimeRanges = gaps
                };
                return Ok(data);
            }
            return NotFound();

        }
    }
}
