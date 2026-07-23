using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ReachSystem.Models;
using ReachSystem.Enums;

namespace ReachSystem.Data
{
    public class ReachSystemDbContext : IdentityDbContext<ApplicationUser>
    {
        public ReachSystemDbContext(DbContextOptions<ReachSystemDbContext> options)
            : base(options)
        {
        }

        public DbSet<Consulta> Consultas { get; set; }
        public DbSet<Animal> Animais { get; set; }
        public DbSet<FichaSaude> FichasSaude { get; set; }
        public DbSet<Evento> Eventos { get; set; }
        public DbSet<Participacao> Participacoes { get; set; }

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

            // Relacionamento: Evento N - N Usuarios
            modelBuilder.Entity<Participacao>()
                .HasOne(p => p.Usuario)
                .WithMany(u => u.Participacoes)
                .HasForeignKey(p => p.UsuarioId);

            modelBuilder.Entity<Participacao>()
                .HasOne(p => p.Evento)
                .WithMany(e => e.Participacoes)
                .HasForeignKey(p => p.EventoId);

            // Enum -> int
            modelBuilder.Entity<Animal>()
                .Property(a => a.SexoAnimal)
                .HasConversion<int>();

            modelBuilder.Entity<Animal>()
                .Property(a => a.StatusAnimal)
                .HasConversion<int>();

            modelBuilder.Entity<Participacao>()
                .Property(p => p.Status)
                .HasConversion<int>();

            //Impede participação duplicada
            modelBuilder.Entity<Participacao>()
                .HasIndex(p => new { p.UsuarioId, p.EventoId })
                .IsUnique();
        }
    }
}
