using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using OrderManagementApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace OrderManagementApp.DAL.Interceptors
{
    public class AuditInterceptor : SaveChangesInterceptor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private List<AuditEntry> _pendingEntries = new();

        public AuditInterceptor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        //before
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData, InterceptionResult<int> result, CancellationToken ct = default)
        {
            if(eventData.Context is not null)
                _pendingEntries = CaptureEntries(eventData.Context);

            return base.SavingChangesAsync(eventData, result, ct);
        }

        //after
        public override async ValueTask<int> SavedChangesAsync(
            SaveChangesCompletedEventData eventData, int result, CancellationToken ct = default)
        {
            if (eventData.Context is null || !_pendingEntries.Any())
                return await base.SavedChangesAsync(eventData, result, ct);

            var auditLogs = new List<AuditLog>();

            foreach(var ae in _pendingEntries)
            {
                var entityId = BuildEntityId(ae.Entry);

                if (ae.IsInsert)
                {
                    ae.NewValues = BuildInsertNewValues(ae.Entry);
                }

                auditLogs.Add(new AuditLog
                {
                    TableName = ae.TableName,
                    EntityId = entityId,
                    Action = ae.Action,
                    OldValues = ae.OldValues,
                    NewValues = ae.NewValues,
                    ChangedColumns = ae.ChangedColumns,
                    ChangedBy = ae.ChangedBy,
                    ChangedAtUtc = ae.ChangedAtUtc
                });
            }

            await eventData.Context.Set<AuditLog>().AddRangeAsync(auditLogs, ct);
            await eventData.Context.SaveChangesAsync(ct);
            _pendingEntries.Clear();

            return await base.SavedChangesAsync(eventData, result, ct);
        }

        private static string BuildEntityId(EntityEntry entityEntry)
        {
            var keyValues = entityEntry.Properties
                                       .Where(p => p.Metadata.IsPrimaryKey())
                                       .ToDictionary(p => p.Metadata.Name, p => p.CurrentValue);

            return JsonSerializer.Serialize(keyValues);
        }

        private static string BuildInsertNewValues(EntityEntry entityEntry)
        {
            var props = new Dictionary<string, Object?>();

            foreach (var p in entityEntry.Properties)
            {
                if (p.Metadata.IsPrimaryKey())
                    continue;

                if (p.Metadata.IsConcurrencyToken)
                    continue;

                object? value;

                if(p.IsTemporary)
                    value = entityEntry.Entity.GetType().GetProperty(p.Metadata.Name)?.GetValue(entityEntry.Entity);
                else
                    value = p.CurrentValue;

                props[p.Metadata.Name] = value;
            }

            return JsonSerializer.Serialize(props);
        }

        private List<AuditEntry> CaptureEntries(DbContext dbContext)
        {
            var entries = new List<AuditEntry>();

            var changedBy = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value ?? "System";
                        
            foreach (var entry in dbContext.ChangeTracker.Entries())
            {
                if (entry.Entity is AuditLog)
                    continue;

                if (entry.State is not (EntityState.Added or EntityState.Modified or EntityState.Deleted))
                    continue;

                var action = entry.State switch
                {
                    EntityState.Added => "Insert",
                    EntityState.Modified => "Update",
                    EntityState.Deleted => "Delete",
                    _ => null
                };

                if (action is null)
                    continue;

                var tableName = entry.Metadata.GetTableName() ?? entry.Entity.GetType().Name;

                var keyProps = entry.Properties
                                    .Where(p => p.Metadata.IsPrimaryKey());
                var entityId = JsonSerializer.Serialize(keyProps.ToDictionary(p => p.Metadata.Name, p => p.CurrentValue));

                string? oldValues = null;
                string? newValues = null;
                string? changedColumns = null;
                    
                if (action == "Update")
                {
                    var oldProps = new Dictionary<string, object?>();
                    var newProps = new Dictionary<string, object?>();
                    var modifiedCols = new List<string>();

                    foreach (var p in entry.Properties)
                    {
                        if (!p.IsModified)
                            continue;

                        modifiedCols.Add(p.Metadata.Name);
                        oldProps[p.Metadata.Name] = p.OriginalValue;
                        newProps[p.Metadata.Name] = p.CurrentValue;
                    }

                    oldValues       = JsonSerializer.Serialize(oldProps);
                    newValues       = JsonSerializer.Serialize(newProps);
                    changedColumns  = JsonSerializer.Serialize(modifiedCols);
                }
                else if (action == "Delete")
                {
                    var props = entry.Properties
                                     .ToDictionary(p => p.Metadata.Name, p => p.OriginalValue);
                    oldValues = JsonSerializer.Serialize(props);
                }

                entries.Add(new AuditEntry
                {
                    Entry = entry,
                    TableName = tableName,
                    Action = action,
                    OldValues = oldValues,
                    NewValues = newValues,
                    ChangedColumns = changedColumns,
                    ChangedBy = changedBy,
                    ChangedAtUtc = DateTimeOffset.UtcNow,
                    IsInsert = action == "Insert"
                });
            }

            return entries;
        }
    }
}
