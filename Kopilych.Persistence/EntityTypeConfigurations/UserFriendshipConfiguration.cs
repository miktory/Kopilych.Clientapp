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
	public class UserFriendshipConfiguration : IEntityTypeConfiguration<UserFriendship>
	{
		public void Configure(EntityTypeBuilder<UserFriendship> builder)
		{
			builder.HasKey(uf => uf.Id);
			builder.HasOne(uf => uf.InitiatorUser).WithMany(u => u.FriendshipsAsInitiator).HasForeignKey(uf => uf.InitiatorUserId).IsRequired();
			builder.HasOne(uf => uf.ApproverUser).WithMany(u => u.FriendshipsAsApprover).HasForeignKey(uf => uf.ApproverUserId).IsRequired();
			builder.HasIndex(u => new { u.InitiatorUserId, u.ApproverUserId }).IsUnique();
            builder.Property(uf => uf.RequestApproved).IsRequired();
			builder.Property(uf => uf.Created).IsRequired();
			builder.Property(uf => uf.Updated).IsRequired();
		}
	}
}
