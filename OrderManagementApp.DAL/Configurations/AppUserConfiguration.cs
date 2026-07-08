using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderManagementApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagementApp.DAL.Configurations
{
    public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder) 
        {
            builder.ToTable("AppUser");
            
            builder.HasKey("Id");

            builder.Property(u => u.Id)
                   .HasColumnName("Id")
                   .HasColumnType("int")
                   .UseIdentityColumn();
            builder.Property(u => u.Username)
                   .HasColumnName("Username")
                   .HasColumnType("varchar(50)")
                   .IsRequired();
            builder.Property(u => u.Email)
                   .HasColumnName("Email")
                   .HasColumnType("varchar(50)")
                   .IsRequired();
            builder.Property(u => u.PasswordHash)
                   .HasColumnName("PasswordHash")
                   .HasColumnType("varchar(256)")
                   .IsRequired();
            builder.Property(u => u.Role)
                   .HasColumnName("Role")
                   .HasColumnType("varchar(50)")
                   .IsRequired();
            builder.Property(u => u.RefreshToken)
                   .HasColumnName("RefreshToken")
                   .HasColumnType("nvarchar(max)");
            builder.Property(u => u.RefreshTokenExpiry)
                   .HasColumnName("RefreshTokenExpiry")
                   .HasColumnType("datetime2(7)");
        }
    }
}
