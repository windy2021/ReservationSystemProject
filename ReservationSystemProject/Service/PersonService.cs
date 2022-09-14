using Microsoft.EntityFrameworkCore;
using ReservationSystemProject.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystemProject.Service
{
    public class PersonService
    {
        private readonly ApplicationDbContext _context;

        public PersonService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Person> UpsertPersonAsync(Person data, bool update)
        {
            var person = await _context.People.FirstOrDefaultAsync(p => p.Email == data.Email);
            if (person == null)
            {
                person = new Person
                {
                    FullName = data.FullName,
                    Email = data.Email,
                    MobileNumber = data.MobileNumber,
                    UserId = data.UserId
                };
                _context.People.Add(person);

            }
            if (person != null && update)
            {
                person.FullName = data.FullName;
                person.Email = data.Email;
                person.MobileNumber = data.MobileNumber;
                person.UserId = data.UserId;
            }
            await _context.SaveChangesAsync();
            return person;
        }
    }
}
