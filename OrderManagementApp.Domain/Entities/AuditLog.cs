using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagementApp.Domain.Entities
{
    public class AuditLog
    {
        public long Id { get; set; }
        public string TableName { get; set; } = string.Empty;
        public string EntityId { get; set; } = string.Empty;  // string because PK types vary (int vs long)
        public string Action { get; set; } = string.Empty;    // Insert / Update / Delete
        public string? OldValues { get; set; }                // JSON snapshot
        public string? NewValues { get; set; }                // JSON snapshot
        public string? ChangedColumns { get; set; }
        public string ChangedBy { get; set; } = string.Empty;
        public DateTimeOffset ChangedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
