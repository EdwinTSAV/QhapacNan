using Microsoft.EntityFrameworkCore;
using QhapaqÑan.Models;

namespace QhapaqÑan.Clases
{
    public class PContext : DbContext
    {
        public PContext(DbContextOptions<PContext> o) : base(o) { }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<Servicios> Servicios { get; set; }
        public DbSet<Hora> Horas { get; set; }
        public DbSet<Reserva> Reservas { get; set; }
        public DbSet<ReservaHora> ReservaHoras { get; set; }
        public DbSet<ReservaServicio> ReservaServicios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new UsuarioMap());
            modelBuilder.ApplyConfiguration(new RolesMap());
            modelBuilder.ApplyConfiguration(new ServiciosMap());
            modelBuilder.ApplyConfiguration(new HoraMap());
            modelBuilder.ApplyConfiguration(new ReservaMap());
            modelBuilder.ApplyConfiguration(new ReservaHoraMap());
            modelBuilder.ApplyConfiguration(new ReservaServicioMap());
        }
    }
}
