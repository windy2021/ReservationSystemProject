using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ReservationSystemProject.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystemProject.Areas.Admin.Controllers
{
    [Area("Admin"), Authorize(Roles = "Manager")]
    public class SittingController : Controller
    {
        private readonly ApplicationDbContext _context;
        //private readonly IMapper _mapper;
        
        public SittingController(ApplicationDbContext context)
        {
            _context = context;
            //_mapper = mapper;
        }
        [HttpGet]
        public IActionResult Index()
        {
            var m = new Models.Sitting.Index();

            m.Sittings = _context.Sittings;
            m.Areas = _context.Areas;
            m.Date = DateTime.Now;
            m.Dates = new SelectList(getDates(), m.Date.Date);

            return View(m);
        }

        [HttpPost]
        public IActionResult Index(Models.Sitting.Index m)
        {
            if (ModelState.IsValid)
            {
                if (m.Date.Ticks == 0)
                {
                    return RedirectToAction("");
                }
                //Return sittings from selected date
                m.Sittings = _context.Sittings
                    .Where(s => s.Start.Date == m.Date.Date);
                m.Dates = new SelectList(getDates(), m.Date.Date);
                m.Areas = _context.Areas;
                return View(m);
            }
            else
            {
                return RedirectToAction("");
            }

        } 

        public IEnumerable<DateTime> getDates()
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

        //SITTINGS
        [HttpGet]
        public IActionResult Create()
        {
            var m = new Models.Sitting.Create();
            m.Date = DateTime.Now.Date;
            return View(m);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Models.Sitting.Create m)
        {
            if (ModelState.IsValid)
            {
                var sitting = new Sitting
                {
                    Name = m.Name,
                    Start = m.Date + m.Start,
                    End = m.Date + m.End,
                    Capacity = m.Capacity,
                    IsPrivate = m.IsPrivate,
                    IsClosed = m.IsClosed,
                    RestaurantId = 1
                };
                _context.Sittings.Add(sitting);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(m);
        }
        
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            var sitting = await _context.Sittings.FirstOrDefaultAsync(s => s.Id == id);
            if(sitting == null)
            {
                return NotFound();
            }

            var m = new Models.Sitting.Edit()
            {
                Id = sitting.Id,
                Name = sitting.Name,
                Date = sitting.Start.Date,
                Start = sitting.Start.TimeOfDay,
                End = sitting.End.TimeOfDay,
                Capacity = sitting.Capacity,
                IsPrivate = sitting.IsPrivate,
                IsClosed = sitting.IsClosed
            };
            return View(m);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Models.Sitting.Edit m)
        {
            if(id != m.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var sitting = await _context.Sittings.FirstOrDefaultAsync(s => s.Id == id);

                sitting.Name = m.Name;
                sitting.Start = m.Date + m.Start;
                sitting.End = m.Date + m.End;
                sitting.Capacity = m.Capacity;
                sitting.IsPrivate = m.IsPrivate;
                sitting.IsClosed = m.IsClosed;

                _context.Update<Sitting>(sitting);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(m);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }
            var sitting = await _context.Sittings.FirstOrDefaultAsync(s => s.Id == id.Value);
            if (sitting == null)
            {
                return NotFound();
            }
            return View(sitting);
        }

        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> Deleted(int? id)
        {
            try
            {
                if (!id.HasValue)
                {
                    return NotFound();
                }

                var sitting = await _context.Sittings.FirstOrDefaultAsync(s => s.Id == id.Value);
                if (sitting == null)
                {
                    return NotFound();
                }
                _context.Sittings.Remove(sitting);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {

            }

            return RedirectToAction(nameof(Delete), new { id });
        }

        //AREAS AND TABLES
        [HttpGet]
        public async Task<IActionResult> ViewTables(int? id)
        {
            var m = new Models.Sitting.ViewTables();

            if (!id.HasValue)
            {
                return NotFound();
            }
            else
            {
                ViewBag.AreaId = id.Value;
            }

            var tables = await _context.Tables.Where(t => t.AreaId == id.Value).ToListAsync();

            if (tables.Count == 0)
            {
                return RedirectToAction(nameof(CreateTable), new { id });
            }

            m.Tables = tables;

            return View(m);
        }

        [HttpGet]
        public IActionResult CreateArea()
        {
            var m = new Models.Sitting.CreateArea();
            return View(m);
        }

        [HttpPost]
        public async Task<IActionResult> CreateArea(Models.Sitting.CreateArea m)
        {
            if (ModelState.IsValid)
            {
                var area = new Area
                {
                    Name = m.Name,
                    Tables = new List<Table> { },
                    RestaurantId = 1
                };
                _context.Areas.Add(area);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(m);
        }

        [HttpGet]
        public IActionResult CreateTable(int? id)
        {
            var m = new Models.Sitting.CreateTable();

            if (!id.HasValue)
            {
                return NotFound();
            }
            else
            {
                m.AreaId = id.Value;
                return View(m);
            }

        }

        [HttpPost]
        public async Task<IActionResult> CreateTable(Models.Sitting.CreateTable m)
        {

            if (ModelState.IsValid)
            {
                var table = new Table
                {
                    Name = m.Name,
                    AreaId = m.AreaId
                };
                _context.Tables.Add(table);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ViewTables));
            }

            return View(m);
        }
        [HttpGet]
        public async Task<IActionResult> DeleteTable(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }
            var table = await _context.Tables.FirstOrDefaultAsync(t => t.Id == id.Value);
            if (table == null)
            {
                return NotFound();
            }
            return View(table);
        }

        [HttpPost]
        [ActionName("DeleteTable")]
        public async Task<IActionResult> DeletedTable(int? id)
        {
            try
            {
                if (!id.HasValue)
                {
                    return NotFound();
                }
                var table = await _context.Tables.FirstOrDefaultAsync(t => t.Id == id.Value);
                if (table == null)
                {
                    return NotFound();
                }
                _context.Tables.Remove(table);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {

            }

            return RedirectToAction(nameof(DeleteTable), new { id });
        }

        [HttpGet]
        public async Task<IActionResult> DeleteArea(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }
            var area = await _context.Areas.FirstOrDefaultAsync(a => a.Id == id.Value);
            if (area == null)
            {
                return NotFound();
            }
            return View(area);
        }

        [HttpPost]
        [ActionName("DeleteArea")]
        public async Task<IActionResult> DeletedArea(int? id)
        {
            try
            {
                if (!id.HasValue)
                {
                    return NotFound();
                }
                var area = await _context.Areas.FirstOrDefaultAsync(a => a.Id == id.Value);
                if (area == null)
                {
                    return NotFound();
                }
                _context.Areas.Remove(area);

                //Delete Tables in Area
                var tables = _context.Tables;
                foreach (var t in tables)
                {
                    if (t.AreaId == id.Value)
                    {
                        _context.Tables.Remove(t);
                    }
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {

            }

            return RedirectToAction(nameof(DeleteArea), new { id });
        }
    }
}
