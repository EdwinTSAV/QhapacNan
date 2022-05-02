using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QhapaqÑan.Models;

namespace QhapaqÑan.Clases
{
    public class HoraMap : IEntityTypeConfiguration<Hora>
    {
        public void Configure(EntityTypeBuilder<Hora> builder)
        {
            builder.ToTable("Hora");
            builder.HasKey(o => o.Id);
        }
    }
}
