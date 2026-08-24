-- Adds the columns required by the account lockout feature to the existing AppUser table.
-- Run this manually against the target database (no EF Core migration history exists in this project).

IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('dbo.AppUser') AND name = 'FailedLoginAttempts'
)
BEGIN
    ALTER TABLE dbo.AppUser ADD FailedLoginAttempts INT NOT NULL CONSTRAINT DF_AppUser_FailedLoginAttempts DEFAULT (0);
END

IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('dbo.AppUser') AND name = 'LockoutEndUtc'
)
BEGIN
    ALTER TABLE dbo.AppUser ADD LockoutEndUtc DATETIME2(7) NULL;
END
