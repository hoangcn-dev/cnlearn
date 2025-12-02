using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Module.Users.Entities;

namespace Module.Users.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.Property(x => x.Email)
                   .HasMaxLength(256);

            builder.Property(x => x.FirstName)
                   .HasMaxLength(100);

            builder.Property(x => x.LastName)
                   .HasMaxLength(100);

            builder.Property(x => x.Password)
                    .HasMaxLength(255);

            builder.Property(x => x.PhoneNumber)
                    .HasMaxLength(15);

            builder.Property(x => x.AvatarUrl)
                   .HasMaxLength(500);

            builder.Property(x => x.Note)
                   .HasMaxLength(500);

            builder.HasIndex(x => x.Email)
                   .IsUnique();

            builder.HasOne(u => u.Role)
                   .WithMany(r => r.Users)
                   .HasForeignKey(u => u.RoleId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
