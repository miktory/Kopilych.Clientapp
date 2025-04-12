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
	public class PiggyBankCustomizationConfiguration : IEntityTypeConfiguration<PiggyBankCustomization>
	{
		public void Configure(EntityTypeBuilder<PiggyBankCustomization> builder)
		{
			builder.HasKey(pbc => pbc.Id);
			builder.Property(pbc => pbc.PhotoPath).IsRequired(false);
			builder.HasOne(pbc => pbc.PiggyBankType).WithMany().HasForeignKey(p => p.PiggyBankTypeId).IsRequired();
			builder.HasOne(pbc => pbc.PiggyBank).WithOne(pb => pb.Customization).HasForeignKey<PiggyBankCustomization>(pbc => pbc.PiggyBankId).IsRequired();

		}
	}
}
