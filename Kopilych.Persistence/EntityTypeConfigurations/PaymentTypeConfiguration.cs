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
	public class PaymentTypeConfiguration : IEntityTypeConfiguration<PaymentType>
	{
		public void Configure(EntityTypeBuilder<PaymentType> builder)
		{
			builder.HasKey(pt => pt.Id);
			builder.Property(pt => pt.Name).HasMaxLength(30).IsRequired();
		}
	}
}
