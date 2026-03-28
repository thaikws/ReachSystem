using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ReachSystem.Models;


namespace ReachSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Animal> Animais { get; set; }
        public DbSet<FichaSaude> FichasSaude { get; set; }

        //Configuração do relacionamento um-para-um entre Animal e FichaSaude
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Animal>()
                .HasOne(a => a.FichaSaude)
                .WithOne(f => f.Animal)
                .HasForeignKey<FichaSaude>(f => f.AnimalId);
        }
    }
}
