using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderManagementApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagementApp.DAL.Configurations
{
    public class ItemConfiguration : IEntityTypeConfiguration<Item>
    {
        public void Configure(EntityTypeBuilder<Item> builder) 
        {
            builder.ToTable("Item");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                   .HasColumnName("Id")
                   .HasColumnType("int")
                   .UseIdentityColumn();

            builder.Property(x => x.ItemName)
                   .HasColumnName("ItemName")
                   .HasColumnType("varchar(100)")
                   .IsRequired();
        }
    }
}
