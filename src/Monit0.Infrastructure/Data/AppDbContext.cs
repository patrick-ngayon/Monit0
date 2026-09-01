using Microsoft.EntityFrameworkCore;
using Monit0.Core.Entities;

namespace Monit0.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }
        public DbSet<Utilisateur> Utilisateurs{get; set;}
        public DbSet<Monitoring> Monitorings{get; set;}
        public DbSet<Application> Applications { get; set;} 
    }
}