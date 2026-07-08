using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderManagementApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagementApp.DAL.Configurations
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable("OrderItem");
            builder.HasKey(i => i.OrderItemId);

            builder.Property(i => i.OrderItemId)
                   .HasColumnName("OrderItemId")
                   .HasColumnType("bigint")
                   .UseIdentityColumn();
            builder.Property(i => i.OrderId)
                   .HasColumnName("OrderId")
                   .HasColumnType("int")
                   .IsRequired();
            builder.Property(i => i.ItemId)
                   .HasColumnName("ItemId")
                   .HasColumnType("int")
                   .IsRequired();
            builder.Property(i => i.UnitPrice)
                   .HasColumnName("UnitPrice")
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();
            builder.Property(i => i.Quantity)
                   .HasColumnName("Quantity")
                   .HasColumnType("int")
                   .IsRequired();
            builder.HasOne(i => i.Order)
                   .WithMany(i => i.OrderItems)
                   .HasForeignKey(i => i.OrderId)
                   .HasConstraintName("FK_OrderItem_Order")
                   .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(i => i.Item)
                   .WithMany(i => i.OrderItems)
                   .HasForeignKey(i => i.ItemId)
                   .HasConstraintName("FK_OrderItem_Item_ItemId")
                   .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable(t =>
            {
                t.HasCheckConstraint("Chk_OrderItem_Quantity", "[Quantity] > 0");
                t.HasCheckConstraint("Chk_OrderItem_UnitPrice", "[UnitPrice] > 0");
            });
        }
    }
}
