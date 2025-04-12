using Kopilych.Domain;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Persistence.EntityTypeConfigurations
{
	public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
	{
		public void Configure(EntityTypeBuilder<Transaction> builder)
		{
			builder.HasKey(t => t.Id);
			builder.Property(t => t.Description).HasMaxLength(200).IsRequired(false);
			builder.HasOne(t => t.User).WithMany(u => u.Transactions).HasForeignKey(t => t.UserId).IsRequired();
			builder.HasOne(t => t.PiggyBank).WithMany().HasForeignKey(t => t.PiggyBankId).IsRequired();
			builder.Property(t => t.Updated).IsRequired();
			builder.Property(t => t.Created).IsRequired();
			builder.Property(t => t.Amount).IsRequired();
			builder.Property(t => t.Date).IsRequired();
			builder.HasOne(t => t.TransactionType).WithMany().HasForeignKey(t => t.TransactionTypeId).IsRequired();
			builder.HasOne(t => t.PaymentType).WithMany().HasForeignKey(t => t.PaymentTypeId).IsRequired();
		}
	}
}
