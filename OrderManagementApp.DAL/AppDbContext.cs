using Microsoft.EntityFrameworkCore;
using OrderManagementApp.DAL.Configurations;
using OrderManagementApp.DAL.Interceptors;
using OrderManagementApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagementApp.DAL
{
    public class AppDbContext : DbContext
    {
        private readonly AuditInterceptor _auditInterceptor;
        public AppDbContext(DbContextOptions options, AuditInterceptor auditInterceptor) : base(options)
        {
            _auditInterceptor = auditInterceptor;
        }

        public DbSet<AppUser> AppUsers => Set<AppUser>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Item> Items => Set<Item>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.AddInterceptors(_auditInterceptor);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AppUserConfiguration());
            modelBuilder.ApplyConfiguration(new CustomerConfiguration());
            modelBuilder.ApplyConfiguration(new ItemConfiguration());
            modelBuilder.ApplyConfiguration(new OrderConfiguration());
            modelBuilder.ApplyConfiguration(new OrderItemConfiguration());
            modelBuilder.ApplyConfiguration(new AuditLogConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
