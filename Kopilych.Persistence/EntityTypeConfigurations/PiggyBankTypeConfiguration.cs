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
	public class PiggyBankTypeConfiguration : IEntityTypeConfiguration<PiggyBankType>
	{
		public void Configure(EntityTypeBuilder<PiggyBankType> builder)
		{
			builder.HasKey(t => t.Id);
			builder.Property(t => t.Name).HasMaxLength(30).IsRequired();
		}
	}
}
