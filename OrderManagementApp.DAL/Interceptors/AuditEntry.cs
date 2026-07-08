using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace OrderManagementApp.DAL.Interceptors
{
    internal sealed class AuditEntry
    {
        public required EntityEntry Entry { get; init; }
        public required string TableName { get; init; }
        public required string Action { get; init; }
        public string? OldValues { get; init; }
        public string? NewValues { get; set; }
        public string? ChangedColumns { get; init; }
        public required string ChangedBy { get; init; }
        public required DateTimeOffset ChangedAtUtc { get; init; }

        public bool IsInsert { get; init; }
    }
}
