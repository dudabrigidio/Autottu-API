using Microsoft.EntityFrameworkCore;
using AutoTTU.Models;

namespace AutoTTU.Connection
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Motos> Motos { get; set; }
        public DbSet<Slot> Slot { get; set; }
        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<Checkin> Checkin { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Ignora explicitamente todas as propriedades booleanas para evitar problemas com Oracle
            modelBuilder.Entity<Motos>().Ignore(e => e.Status);
            modelBuilder.Entity<Slot>().Ignore(e => e.Ocupado);
            modelBuilder.Entity<Checkin>().Ignore(e => e.Violada);
        }
    }
}