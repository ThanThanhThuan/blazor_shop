using AccountingApp.Shared.Models;

using System.Collections.Generic;
using System.Reflection.Emit;
using System.Security.Principal;
using AccountingApp.Data;
using Microsoft.EntityFrameworkCore;

namespace AccountingApp.Data // Note the namespace might differ slightly
{
    public class AccountingContext : DbContext
    {
        public AccountingContext(DbContextOptions<AccountingContext> options) : base(options) { }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<JournalEntry> JournalEntries { get; set; }
        public DbSet<JournalEntryLine> JournalEntryLines { get; set; }
        public DbSet<JournalEntryAttachment> Attachments { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>().HasData(
                new Account { Id = 1, Name = "Cash", Type = "Asset" },
                new Account { Id = 2, Name = "Sales Revenue", Type = "Income" }
            );
            modelBuilder.Entity<Product>().HasData(
           new Product { Id = 1, Name = "Laptop", Price = 999.99m, Stock = 10 },
           new Product { Id = 2, Name = "Mouse", Price = 25.50m, Stock = 50 },
           new Product { Id = 3, Name = "Keyboard", Price = 45.00m, Stock = 0 } // Out of stock example
       );
        }
    }
}