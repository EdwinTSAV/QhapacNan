using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QhapaqÑan.Clases;

namespace QhapaqÑan.Models
{
    public class ServiciosMap : IEntityTypeConfiguration<Servicios>
    {
        public void Configure(EntityTypeBuilder<Servicios> builder)
        {
            builder.ToTable("Servicios");
            builder.HasKey(o => o.Id);
        }
    }
}
