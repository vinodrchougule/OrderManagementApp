using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderManagementApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagementApp.DAL.Configurations
{
    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.ToTable("AuditLog");
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id)
                   .HasColumnName("Id")
                   .HasColumnType("bigint")
                   .UseIdentityColumn();   // bigint identity

            builder.Property(a => a.TableName)
                   .HasColumnName("TableName")
                   .HasColumnType("varchar(128)")
                   .IsRequired();

            builder.Property(a => a.EntityId)
                   .HasColumnName("EntityId")
                   .HasColumnType("varchar(128)")
                   .IsRequired();

            builder.Property(a => a.Action)
                   .HasColumnName("Action")
                   .HasColumnType("varchar(20)")
                   .IsRequired();

            builder.Property(a => a.OldValues)
                   .HasColumnName("OldValues")
                   .HasColumnType("nvarchar(max)");

            builder.Property(a => a.NewValues)
                   .HasColumnName("NewValues")
                   .HasColumnType("nvarchar(max)");

            builder.Property(a => a.ChangedColumns)
                   .HasColumnName("ChangedColumns")
                   .HasColumnType("nvarchar(max)");

            builder.Property(a => a.ChangedBy)
                   .HasColumnName("ChangedBy")
                   .HasColumnType("varchar(50)");   // nullable per script

            builder.Property(a => a.ChangedAtUtc)
                   .HasColumnName("ChangedAtUtc")
                   .HasColumnType("datetimeoffset(7)")
                   .IsRequired();
        }
    }
}
