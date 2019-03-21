using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MonitorWeb.Domain;

namespace MonitorWeb.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {

        public DbSet<Host> Hosts { get; set; }
        
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}