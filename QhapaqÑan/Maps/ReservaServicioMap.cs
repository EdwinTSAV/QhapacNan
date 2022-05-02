using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QhapaqÑan.Models;

namespace QhapaqÑan.Clases
{
    public class ReservaServicioMap : IEntityTypeConfiguration<ReservaServicio>
    {
        public void Configure(EntityTypeBuilder<ReservaServicio> builder)
        {
            builder.ToTable("ReservaServicio");
            builder.HasKey(o => o.Id);

            builder.HasOne(o => o.Servicios).
                WithMany().
                HasForeignKey(o => o.Id_Servicio);
            builder.HasOne(o => o.Reserva).
                WithMany().
                HasForeignKey(o => o.Id_Reserva);
        }
    }
}
