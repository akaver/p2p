using System;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class AppDbContext: DbContext
    {

        public DbSet<Host> Hosts { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
        {
        }
    }
}