using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QhapaqÑan.Models;

namespace QhapaqÑan.Clases
{
    public class ReservaHoraMap : IEntityTypeConfiguration<ReservaHora>
    {
        public void Configure(EntityTypeBuilder<ReservaHora> builder)
        {
            builder.ToTable("ReservaHora");
            builder.HasKey(o => o.Id);

            builder.HasOne(o => o.Hora).
                WithMany().
                HasForeignKey(o => o.Id_Hora);
            builder.HasOne(o => o.Reserva).
                WithMany().
                HasForeignKey(o => o.Id_Reserva);
        }
    }
}
