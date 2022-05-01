using Microsoft.EntityFrameworkCore;
using QhapaqÑan.Clases;

namespace QhapaqÑan.Models
{
    public class PContext : DbContext
    {
        public PContext(DbContextOptions<PContext> o) : base(o) { }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<Servicios> Servicios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new UsuarioMap());
            modelBuilder.ApplyConfiguration(new RolesMap());
            modelBuilder.ApplyConfiguration(new ServiciosMap());
        }
    }
}
