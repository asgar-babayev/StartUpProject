using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StartUpProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StartUpProject.DAL
{
    public class Context : IdentityDbContext<AppUser>
    {
        public Context(DbContextOptions<Context> options):base(options)
        {

        }

        public DbSet<Testimonial> Testimonials { get; set; }
    }
}
