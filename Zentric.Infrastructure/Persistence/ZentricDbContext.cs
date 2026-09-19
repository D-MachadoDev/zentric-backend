using Microsoft.EntityFrameworkCore;
using Zentric.Domain.Orders;
using Zentric.Domain.Logistics;
using Zentric.Domain.Returns;
using Zentric.Domain.Billing;
using Zentric.Domain.Users;
using Zentric.Domain.Buyers;
using Zentric.Domain.Products;
using Zentric.Domain.Inventories;
using Zentric.Domain.Warehouses;
using Zentric.Domain.Users.ValueObjects;

namespace Zentric.Infrastructure.Persistence
{
    public class ZentricDbContext : DbContext
    {
        public DbSet<CustomerOrder> CustomerOrders => Set<CustomerOrder>();
        public DbSet<FulfillmentOrder> FulfillmentOrders => Set<FulfillmentOrder>();
        public DbSet<ReturnRequest> ReturnRequests => Set<ReturnRequest>();
        public DbSet<Invoice> Invoices => Set<Invoice>();
        
        public DbSet<User> Users => Set<User>();
        public DbSet<Buyer> Buyers => Set<Buyer>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Inventory> Inventories => Set<Inventory>();
        public DbSet<Warehouse> Warehouses => Set<Warehouse>();

        public ZentricDbContext(DbContextOptions<ZentricDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<CustomerOrder>(b =>
            {
                b.HasKey(o => o.Id);
                b.OwnsMany(o => o.Items, i =>
                {
                    i.WithOwner().HasForeignKey(x => x.CustomerOrderId);
                    i.HasKey(x => x.Id);
                    i.OwnsOne(x => x.UnitPrice);
                });
            });

            modelBuilder.Entity<FulfillmentOrder>(b =>
            {
                b.HasKey(f => f.Id);
                b.OwnsMany(f => f.Shipments, s =>
                {
                    s.WithOwner().HasForeignKey(x => x.FulfillmentOrderId);
                    s.HasKey(x => x.Id);
                });
            });
            
            modelBuilder.Entity<Invoice>(b => 
            {
                b.HasKey(i => i.Id);
                b.OwnsOne(i => i.TotalAmount);
            });

            modelBuilder.Entity<User>(b => 
            {
                b.HasKey(u => u.Id);
                b.OwnsOne(u => u.FullName);
                b.Property(u => u.Email).HasConversion(e => e.Value, v => new Email(v));
            });

            modelBuilder.Entity<Buyer>().HasKey(b => b.UserId);
            modelBuilder.Entity<Inventory>().HasKey(i => i.Id);
            modelBuilder.Entity<Warehouse>().HasKey(w => w.Id);
            
            modelBuilder.Entity<Product>(b =>
            {
                b.HasKey(p => p.Id);
                b.OwnsOne(p => p.Price);
                b.OwnsMany(p => p.Variants, v => 
                {
                    v.WithOwner().HasForeignKey(x => x.ProductId);
                    v.HasKey(x => x.Id);
                    v.OwnsMany(x => x.Attributes, a => 
                    {
                        a.WithOwner().HasForeignKey("VariantId");
                        a.HasKey("VariantId", "Name");
                    });
                });
            });
        }
    }
}

