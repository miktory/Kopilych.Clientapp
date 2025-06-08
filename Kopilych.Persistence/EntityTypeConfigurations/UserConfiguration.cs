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
	public class UserConfiguration : IEntityTypeConfiguration<User>
	{
		public void Configure(EntityTypeBuilder<User> builder) 
		{
			builder.HasKey(u => u.Id);
			builder.HasIndex(u => u.ExternalId).IsUnique();
			builder.Property(u => u.Username).HasMaxLength(50).IsRequired();
			builder.Property(u => u.Updated).IsRequired();
			builder.Property(u => u.Created).IsRequired();
			builder.Property(u => u.PhotoPath).IsRequired(false);

		}
	}
}
