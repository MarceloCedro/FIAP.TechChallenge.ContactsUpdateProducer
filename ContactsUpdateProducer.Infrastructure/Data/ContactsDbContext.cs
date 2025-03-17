using FIAP.TechChallenge.ContactsUpdateProducer.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FIAP.TechChallenge.ContactsUpdateProducer.Infrastructure.Data
{
    public class ContactsDbContext : DbContext
    {
        public ContactsDbContext(DbContextOptions<ContactsDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseMySql("DefaultConnection",
                                        new MySqlServerVersion(new Version(8, 0, 21)),
                                        mySqlOptions => mySqlOptions.MigrationsAssembly("FIAP.TechChallenge.ContactsUpdateProducer.Infrastructure"));
        }

        public DbSet<Contact> Contacts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Contact>(entity =>
            {
                entity.ToTable("Contacts");

                // Configuração da chave primária
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                // Configuração das propriedades
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasMaxLength(15);

                entity.Property(e => e.AreaCode)
                    .IsRequired()
                    .HasMaxLength(2);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasIndex(e => e.Email)
                    .IsUnique();
            });
        }
    }
}