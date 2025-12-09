using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Module.Users.Entities;

namespace Module.Users.Configurations
{
    public class UserLogConfiguration : IEntityTypeConfiguration<UserLog>
    {
        public void Configure(EntityTypeBuilder<UserLog> builder)
        {
            builder.ToTable("UserLogs");

            builder.Property(x => x.Log)
                   .HasMaxLength(100);

            builder.HasOne(ul => ul.User)
                .WithMany(u => u.Logs)
                .HasForeignKey(ul => ul.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}