-- Adds the columns required by the Forgot Password / Reset Password feature to the existing AppUser table.
-- Run this manually against the target database (no EF Core migration history exists in this project).

IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('dbo.AppUser') AND name = 'PasswordResetToken'
)
BEGIN
    ALTER TABLE dbo.AppUser ADD PasswordResetToken NVARCHAR(MAX) NULL;
END

IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('dbo.AppUser') AND name = 'PasswordResetTokenExpiry'
)
BEGIN
    ALTER TABLE dbo.AppUser ADD PasswordResetTokenExpiry DATETIME2(7) NULL;
END
