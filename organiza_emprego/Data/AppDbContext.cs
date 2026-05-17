using organiza_emprego.Models;
using Microsoft.EntityFrameworkCore;

namespace organiza_emprego.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Candidatura> Candidaturas { get; set; }

        public DbSet<Usuario> Usuarios { get; set; }
    }
}
