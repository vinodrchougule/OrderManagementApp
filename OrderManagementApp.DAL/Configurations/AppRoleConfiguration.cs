using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderManagementApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagementApp.DAL.Configurations
{
    public class AppRoleConfiguration : IEntityTypeConfiguration<AppRole>
    {
        public void Configure(EntityTypeBuilder<AppRole> builder)
        {
            builder.ToTable("AppRole");

            builder.HasKey(r => r.AppRoleId);

            builder.Property(r => r.AppRoleId)
                   .HasColumnName("AppRoleId")
                   .HasColumnType("bigint")
                   .UseIdentityColumn();

            builder.Property(r => r.RoleName)
                   .HasColumnName("RoleName")
                   .HasColumnType("varchar(50)")
                   .IsRequired();

            builder.HasIndex(r => r.RoleName)
                   .IsUnique()
                   .HasDatabaseName("UK_AppRole_RoleName");
        }
    }
}
