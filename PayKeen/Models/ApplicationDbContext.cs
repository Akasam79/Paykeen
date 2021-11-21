using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PayKeen.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
        {

        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ActivePayments> ActivePayments { get; set; }
        public DbSet<Customers> Customers { get; set; }
        public DbSet<CWallet> CWallet { get; set; }

        public DbSet<Merchant> Merchants { get; set; }

        public DbSet<MWallet> MWallet { get; set; }

        public DbSet<OutletAdmins> outletAdmins { get; set; }
        public DbSet<Payments> Payments { get; set; }

        public DbSet<TopUps> TopUps { get; set; }

        public DbSet<Users> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB; Initial Catalog=SciPay;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Users>()
            .HasIndex(u => u.Phone)
            .IsUnique();
        }
    }
}
