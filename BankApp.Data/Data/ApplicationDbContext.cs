using BankApp.Data.Enums;
using BankApp.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BankApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }
        
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Transaction>()
                .Property(t => t.TransactionAmount)
                .HasPrecision(18, 2);

            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Account>().HasData(
                new Account
                {
                    Id = 1,
                    FirstName = "YouBank",
                    LastName = "settlement Account",
                    AccountType = (AccountType)3,
                    PhoneNumber = "08035064624",
                    Email = "settlement@youbank.com",
                    AccountNumberGenerated = "9053769810",
                    CurrentAccountBalance = "999999999",
                    DateCreated = DateTime.Now,
                    DateLastUpdated = DateTime.Now,
                });
        }

    }
}
