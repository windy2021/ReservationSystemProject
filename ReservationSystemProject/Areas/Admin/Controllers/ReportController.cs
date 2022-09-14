using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReservationSystemProject.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystemProject.Areas.Admin.Controllers
{
    [Area("Admin"), Authorize(Roles = "Manager")]
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewData["Count"] = getCount();
            ViewData["Months"] = getMonths();

            return View();
        }

        public List<DateTime> getDates()
        {
            
            try
            {
                DateTime temp = _context.Reservations.First().Start.Date;
                List<DateTime> dates = new List<DateTime>() { temp };
                foreach (var r in _context.Reservations)
                {
                    if (temp.CompareTo(r.Start.Date) < 0)
                    {
                        dates.Add(r.Start.Date);
                        temp = r.Start.Date;
                    }
                }
                return dates;
            }
            catch(Exception e)
            {
                List<DateTime> test = new List<DateTime>();
                Debug.WriteLine(e.Message);
                return test;
            }
        }

        public List<string> getMonths()
        {
            List<DateTime> dates = getDates();
            List<string> months = new List<string>();
            int temp = 0;

            foreach (var d in dates)
            {
                if (temp.CompareTo(d.Month) < 0)
                {
                    months.Add(d.Date.ToString("MMMM"));
                    temp = d.Month;
                }
            }

            return months;
        }

        public List<int> getCount()
        {
            List<int> countArray = new List<int>();

            for (int i = 0; i < 13; i++)
            {
                int count = 0;

                foreach (var r in _context.Reservations)
                {
                    if (r.Start.Date.Month == i)
                    {
                        count++;
                    }
                }

                if (count > 0)
                {
                    countArray.Add(count);
                }
            }

            return countArray;
        }

    }
}
