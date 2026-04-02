using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ReachSystem.Models;

namespace ReachSystem.Data
{
    public class ReachSystemDbContext : IdentityDbContext
    {
        public ReachSystemDbContext(DbContextOptions<ReachSystemDbContext> options)
             : base(options)
        {
        }
        public DbSet<Consulta> Consultas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Configuração do relacionamento um-para-n entre Animal e Consulta
            modelBuilder.Entity<Consulta>()
                .HasOne(c => c.Animal)
                .WithMany(a => a.Consultas)
                .HasForeignKey(c => c.AnimalId);
        }

    }
}
