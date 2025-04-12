using Kopilych.Domain;
using Kopilych.Persistence.EntityTypeConfigurations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Persistence
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
		public DbSet<User> Users { get; set; }
		public DbSet<PiggyBank> PiggyBanks { get; set; }
		public DbSet<UserFriendship> UserFriendships { get; set; }
		public DbSet<Transaction> Transactions { get; set; }
		public DbSet<TransactionType> TransactionTypes { get; set; }
		public DbSet<PaymentType> PaymentTypes { get; set; }
        public DbSet<PremiumUser> PremiumUsers { get; set; }
        public DbSet<UserPiggyBank> UserPiggyBanks { get; set; }
		public DbSet<PiggyBankCustomization> PiggyBankCustomizations { get; set; }
        public DbSet<PiggyBankType> PiggyBankTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.ApplyConfiguration(new PiggyBankConfiguration());
			modelBuilder.ApplyConfiguration(new PiggyBankCustomizationConfiguration());
			modelBuilder.ApplyConfiguration(new UserConfiguration());
			modelBuilder.ApplyConfiguration(new UserFriendshipConfiguration());
			modelBuilder.ApplyConfiguration(new UserPiggyBankConfiguration());
			modelBuilder.ApplyConfiguration(new TransactionConfiguration());
			modelBuilder.ApplyConfiguration(new TransactionTypeConfiguration());
			modelBuilder.ApplyConfiguration(new PaymentTypeConfiguration());
			modelBuilder.ApplyConfiguration(new PiggyBankTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PremiumUserConfiguration());

            base.OnModelCreating(modelBuilder);
		}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
            // другие настройки
        }
    }
}
