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
	public class UserPiggyBankConfiguration : IEntityTypeConfiguration<UserPiggyBank>
	{
		public void Configure(EntityTypeBuilder<UserPiggyBank> builder)
		{
			builder.HasKey(upb => upb.Id);
			builder.HasOne(upb => upb.User).WithMany(u => u.UserPiggyBankRecords).HasForeignKey(upb => upb.UserId);
			builder.Property(upb => upb.Public).IsRequired();
			builder.Property(upb => upb.HideBalance).IsRequired();
			builder.HasOne(upb => upb.PiggyBank).WithMany(pb => pb.Members).IsRequired().HasForeignKey(upb => upb.PiggyBankId);
			builder.Property(upb => upb.Updated).IsRequired();
			builder.Property(upb => upb.Created).IsRequired();
		}
	}
}
