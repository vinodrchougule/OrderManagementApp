using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OrderManagementApp.Domain.Entities;
using OrderManagementApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagementApp.DAL.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Order");

            builder.HasKey("OrderId");

            builder.Property(e => e.OrderId)
                   .HasColumnName("OrderId")
                   .HasColumnType("int")
                   .UseIdentityColumn();
            builder.Property(e => e.OrderDate)
                   .HasColumnName("OrderDate")
                   .HasColumnType("DateTime")
                   .IsRequired();
            builder.Property(e => e.CustomerId)
                   .HasColumnName("CustomerId")
                   .HasColumnType("int")
                   .IsRequired();
            builder.Property(e => e.TotalAmount)
                   .HasColumnName("TotalAmount")
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();
            builder.Property(e => e.Status)
                   .HasColumnName("Status")
                   .HasColumnType("varchar(20)")
                   .HasConversion(
                        v => v.ToString(),
                        v => Enum.Parse<OrderStatus>(v))
                   .IsRequired();
            builder.Property(e => e.RowVersion)
                   .HasColumnName("RowVersion")
                   .IsRowVersion();

            //Foregin Keys
            builder.HasOne(o => o.Customer)
                   .WithMany(o => o.Orders)
                   .HasForeignKey(o => o.CustomerId)
                   .HasConstraintName("FK_Order_Customer_CustomerId")
                   .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable(t => t.HasCheckConstraint("Chk_TotalAmount", "[TotalAmount] > 0"));
        }
    }
}
