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
    public class PremiumUserConfiguration: IEntityTypeConfiguration<PremiumUser>
    {
        public void Configure(EntityTypeBuilder<PremiumUser> builder)
        {
            builder.HasKey(pu => pu.Id);
            builder.HasOne(p => p.User)
              .WithOne()
              .HasForeignKey<PremiumUser>(p => p.UserId).IsRequired();
        }
    }
}
