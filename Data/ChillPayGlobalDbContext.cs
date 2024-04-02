using ChillPay.Merchant.Register.Api.Entities.Customers;
using ChillPay.Merchant.Register.Api.Entities.Generics;
using ChillPay.Merchant.Register.Api.Entities.Registers;
using ChillPay.Merchant.Register.Api.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace ChillPay.Merchant.Register.Api.Data
{
    public class ChillPayGlobalDbContext : DbContext
    {
        public ChillPayGlobalDbContext(DbContextOptions<ChillPayGlobalDbContext> options) : base(options) { }

        public ChillPayGlobalDbContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //Trusted_Connection=True;
                optionsBuilder.UseSqlServer("",
                    builder => builder.EnableRetryOnFailure());
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            ////Users
            //var user = builder.Entity<Customer>().ToTable("Users");

            //builder.Entity<Role>().ToTable("Roles");
            //builder.Entity<UserClaim>().ToTable("UserClaims");

            //builder.Entity<UserRole>(b =>
            //{
            //    b.HasKey(m => new { m.UserId, m.RoleId });
            //    b.ToTable("UserRoles");
            //});

            //builder.Entity<UserLogin>(b =>
            //{
            //    b.HasKey(m => new { m.LoginProvider, m.ProviderKey, m.UserId });
            //    b.ToTable("UserLogins");
            //});
            //builder.Entity<RoleClaim>().ToTable("RoleClaims");
            //builder.Entity<UserMerchantMapping>().ToTable("UserMerchantMappings"); //UserMerchantMapping
            //builder.Entity<CustomerProfile>().ToTable("UserProfiles");

            ////Generics
            //builder.Entity<Contact>().ToTable("Contacts");
            builder.Entity<Province>().ToTable("Province");
            builder.Entity<District>().ToTable("Districts");
            builder.Entity<Subdistrict>().ToTable("Subdistricts");

            ////RegisterMerchant
            builder.Entity<RegisterAddress>().ToTable("RegisterAddresses");
            builder.Entity<RegisterBankAccountType>().ToTable("RegisterBankAccountTypes");
            builder.Entity<RegisterBank>().ToTable("RegisterBanks");
            //builder.Entity<RegisterChannelBaseFee>().ToTable("RegisterChannelBaseFees");
            //builder.Entity<RegisterChannelBaseServiceFee>().ToTable("RegisterChannelBaseServiceFees");
            builder.Entity<RegisterChannelCondition>().ToTable("RegisterChannelConditions");
            builder.Entity<RegisterChannelFee>().ToTable("RegisterChannelFees");
            builder.Entity<RegisterChannel>().ToTable("RegisterChannels");
            builder.Entity<RegisterChannelServiceFee>().ToTable("RegisterChannelServiceFees");

            builder.Entity<RegisterContact>().ToTable("RegisterContacts");
            //builder.Entity<RegisterDataChangeLog>().ToTable("RegisterDataChangeLogs");
            //builder.Entity<RegisterDocument>().ToTable("RegisterDocuments");
            builder.Entity<RegisterInstallment>().ToTable("RegisterInstallments");
            builder.Entity<RegisterMerchant>().ToTable("RegisterMerchants");
            builder.Entity<RegisterStateLog>().ToTable("RegisterStateLogs");

        }

        //public DbSet<Customer> Users { get; set; }
        //public DbSet<UserRole> UserRoles { get; set; }
        //public DbSet<RoleClaim> RoleClaims { get; set; }
        //public DbSet<UserMerchantMapping> UserMerchantMappings { get; set; }
        //public DbSet<CustomerProfile> UserProfiles { get; set; }

        //public DbSet<Contact> Contacts { get; set; }
        public DbSet<Province> Provinces { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<Subdistrict> Subdistricts { get; set; }

        public DbSet<RegisterAddress> RegisterAddresses { get; set; }
        public DbSet<RegisterBankAccountType> RegisterBankAccountTypes { get; set; }
        public DbSet<RegisterBank> RegisterBanks { get; set; }
        //public DbSet<RegisterChannelBaseFee> RegisterChannelBaseFees { get; set; }
        //public DbSet<RegisterChannelBaseServiceFee> RegisterChannelBaseServiceFees { get; set; }
        public DbSet<RegisterChannelCondition> RegisterChannelConditions { get; set; }
        public DbSet<RegisterChannelFee> RegisterChannelFees { get; set; }
        public DbSet<RegisterChannel> RegisterChannels { get; set; }
        public DbSet<RegisterChannelServiceFee> RegisterChannelServiceFees { get; set; }
        public DbSet<RegisterContact> RegisterContacts { get; set; }
        //public DbSet<RegisterDataChangeLog> RegisterDataChangeLogs { get; set; }
        //public DbSet<RegisterDocument> RegisterDocuments { get; set; }
        public DbSet<RegisterInstallment> RegisterInstallments { get; set; }
        public DbSet<RegisterMerchant> RegisterMerchants { get; set; }
        public DbSet<RegisterStateLog> RegisterStateLogs { get; set; }

    }
}
