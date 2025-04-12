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
	public class PiggyBankConfiguration : IEntityTypeConfiguration<PiggyBank>
	{
		public void Configure(EntityTypeBuilder<PiggyBank> builder)
		{
			builder.HasKey(pb => pb.Id);
			builder.Property(pb => pb.Name).HasMaxLength(100).IsRequired();
			builder.HasOne(pb => pb.Owner).WithMany(u => u.PiggyBanks).HasForeignKey(p => p.OwnerId).IsRequired();
			builder.Property(pb => pb.Balance).IsRequired();
			builder.Property(pb => pb.Goal).IsRequired(false);
			builder.Property(pb => pb.GoalDate).IsRequired(false);
			builder.Property(pb => pb.Description).HasMaxLength(300).IsRequired(false);
			builder.Property(pb => pb.Shared).IsRequired();
			builder.Property(pb => pb.Created).IsRequired();
			builder.Property(pb => pb.Updated).IsRequired();
		}
	}
}
