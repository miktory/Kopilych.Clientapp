using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kopilych.Domain;

namespace Kopilych.Persistence.EntityTypeConfigurations
{
	public class TransactionTypeConfiguration : IEntityTypeConfiguration<TransactionType>
	{
		public void Configure(EntityTypeBuilder<TransactionType> builder)
		{
			builder.HasKey(t => t.Id);
			builder.Property(t => t.Name).HasMaxLength(30).IsRequired();
		}
	}
}
