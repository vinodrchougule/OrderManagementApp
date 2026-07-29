using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderManagementApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagementApp.DAL.Configurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable("Customer");
            
            builder.HasKey(c => c.Id);
            
            builder.Property(c => c.Id)
                   .HasColumnName("Id")
                   .HasColumnType("int")
                   .UseIdentityColumn();

            builder.Property(c => c.CustomerName)
                    .HasColumnName("CustomerName")
                    .HasColumnType("varchar(100)")
                    .IsRequired();

            builder.HasIndex(c => c.CustomerName)
                   .IsUnique()
                   .HasDatabaseName("UQ_Customer_CustomerName");
        }
    }
}
