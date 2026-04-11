using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ReachSystem.Models;
using ReachSystem.Enums;

namespace ReachSystem.Data
{
    public class ReachSystemDbContext : IdentityDbContext
    {
        public ReachSystemDbContext(DbContextOptions<ReachSystemDbContext> options)
            : base(options)
        {
        }

        public DbSet<Consulta> Consultas { get; set; }
        public DbSet<Animal> Animais { get; set; }
        public DbSet<FichaSaude> FichasSaude { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Relacionamento: Animal 1 - N Consultas
            modelBuilder.Entity<Consulta>()
                .HasOne(c => c.Animal)
                .WithMany(a => a.Consultas)
                .HasForeignKey(c => c.AnimalId);

            // Relacionamento: Animal 1 - 1 FichaSaude
            modelBuilder.Entity<Animal>()
                .HasOne(a => a.FichaSaude)
                .WithOne(f => f.Animal)
                .HasForeignKey<FichaSaude>(f => f.AnimalId);

            // Enum -> int
            modelBuilder.Entity<Animal>()
                .Property(a => a.SexoAnimal)
                .HasConversion<int>();

            modelBuilder.Entity<Animal>()
                .Property(a => a.StatusAnimal)
                .HasConversion<int>();
        }
    }
}
