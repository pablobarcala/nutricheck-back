using Microsoft.EntityFrameworkCore;
using NutriCheck.Models;

namespace NutriCheck.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Paciente> Pacientes { get; set; }
    }
}
