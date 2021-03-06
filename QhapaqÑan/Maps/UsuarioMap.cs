using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QhapaqÑan.Models;

namespace QhapaqÑan.Clases
{
    public class UsuarioMap : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.ToTable("Usuario");
            builder.HasKey(o => o.DNI);

            builder.HasOne(o => o.Roles).
                WithMany().
                HasForeignKey(o => o.Id_Rol);
        }
    }
}
