using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
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
    [Area("Admin"), Authorize(Roles = "Manager")]
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly PersonService _personService;
        private readonly SignInManager<IdentityUser> _signInManager;
        public EmployeeController(ApplicationDbContext context, UserManager<IdentityUser> userManager, PersonService personService, SignInManager<IdentityUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _personService = personService;
            _signInManager = signInManager;
        }
        // GET: EmployeeController
        public ActionResult Index()
        {
            List<Person> personList = new List<Person>(); //all registered person without any role
            var people = _context.People;

            foreach (var person in people)
            {
                if (person.UserId != null)
                {
                    personList.Add(person);
                }
            }

            var m = new Models.Employee.Index
            {
                People = personList
            };

            return View(m);
        }

        // GET: EmployeeController/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            try
            {
                if (!id.HasValue)
                {
                    return StatusCode(400, "Id required");
                }
                var person = await _context.People.FirstOrDefaultAsync(p => p.Id == id.Value);
                if (person == null)
                {
                    return NotFound();
                }
                var m = new Models.Employee.Details
                {
                    Id = person.Id,
                    FullName = person.FullName,
                    MobileNumber = person.MobileNumber,
                    Email = person.Email,
                    Role = GetUserRole(person.UserId)
                };
                return View(m);
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        // GET: EmployeeController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EmployeeController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: EmployeeController/Edit/5
        public async Task<ActionResult> UpdateFromIndex(int? id)
        {
            var person = await _context.People.FirstOrDefaultAsync(p => p.Id == id);
            return RedirectToAction("Update", new { id = id, role = GetUserRole(person.UserId) });
        }
        public async Task<ActionResult> Update(int? id, string role)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }
            var person = await _context.People.FirstOrDefaultAsync(p => p.Id == id);
            if (person == null)
            {
                return NotFound();
            }
            var m = new Models.Employee.Update
            {
                FullName = person.FullName,
                MobileNumber = person.MobileNumber,
                Email = person.Email,
                Role = role
            };

            m.Roles = new SelectList(GetAllRoles().ToArray());
            return View(m);
        }

        // POST: EmployeeController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, Models.Employee.Update m)
        {
            if (ModelState.IsValid)
            {
                if (id != m.Id)
                {
                    return NotFound();
                }
                var person = _context.People.FirstOrDefault(p => p.Id == id);
                var user = await _userManager.FindByIdAsync(person.UserId);
                var roles = await _userManager.GetRolesAsync(user);
                var result = await _userManager.RemoveFromRolesAsync(user, roles.ToArray());
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, m.Role);
                }
                var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
                if (m.MobileNumber != phoneNumber)
                {
                    var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, m.MobileNumber);
                    person.MobileNumber = m.MobileNumber;
                    await _context.SaveChangesAsync();
                }
                var fullName = person.FullName;
                if (m.FullName != fullName)
                {
                    person.FullName = m.FullName;
                    _context.People.Update(person);
                    await _context.SaveChangesAsync();
                }
                await _signInManager.RefreshSignInAsync(await _userManager.GetUserAsync(User));
                return RedirectToAction("Details", new { id = id });
            }
            else
            {
                m.Roles = new SelectList(GetAllRoles().ToArray());
                return View(m);
            }
        }

        // GET: EmployeeController/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (!id.HasValue)
                {
                    return StatusCode(400, "Id required");
                }
                var person = await _context.People.FirstOrDefaultAsync(p => p.Id == id.Value);
                if (person == null)
                {
                    return NotFound();
                }
                var m = new Models.Employee.Details
                {
                    Id = person.Id,
                    FullName = person.FullName,
                    MobileNumber = person.MobileNumber,
                    Email = person.Email,
                    Role = GetUserRole(person.UserId)
                };
                return View(m);
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        // POST: EmployeeController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, Models.Employee.Details m)
        {
            if (id != m.Id)
            {
                return NotFound();
            }
            var person = _context.People.FirstOrDefault(p => p.Id == id);
            var user = await _userManager.FindByIdAsync(person.UserId);
            IdentityResult result = null;
            if (user != null && person !=null)
            {
                _context.People.Remove(person);
                await _context.SaveChangesAsync();
                result = await _userManager.DeleteAsync(user);
            }
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }
        public string GetUserRole(string userId)
        {
            var user = _context.UserRoles.FirstOrDefault(user => user.UserId == userId);
            if (user != null)
            {
                var role = _context.Roles.FirstOrDefault(role => role.Id == user.RoleId);
                return role.Name;
            }
            else return "No Role";
        }
        public List<string> GetAllRoles()
        {
            List<string> roles = new List<string>();
            var allRoles = _context.Roles;
            foreach (var role in allRoles)
            {
                roles.Add(role.Name);
            }
            return roles;
        }
    }
}
