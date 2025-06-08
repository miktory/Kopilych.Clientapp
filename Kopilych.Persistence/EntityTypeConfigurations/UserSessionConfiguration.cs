using Kopilych.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Persistence.EntityTypeConfigurations
{
    public class UserSessionConfiguration: IEntityTypeConfiguration<UserSession>
    {
        public void Configure(EntityTypeBuilder<UserSession> builder)
        {
            builder.HasKey(us => us.Id);
            builder.HasOne(us => us.User).WithOne(u => u.Session).HasForeignKey<UserSession>(us => us.UserId).IsRequired();
            builder.Property(us => us.AccessToken).IsRequired();
            builder.Property(us => us.RefreshToken).IsRequired();
        }
    }
}
