-- After INSERT trigger for dbo.tstUser.
-- Logs inserted rows into dbo.tstUser_Audit.
-- Adjust audit table column types to match dbo.tstUser.

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID('dbo.tstUser_Audit', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.tstUser_Audit (
        AuditId INT IDENTITY(1, 1) NOT NULL
            CONSTRAINT PK_tstUser_Audit PRIMARY KEY,
        Id INT NOT NULL,
        Name NVARCHAR(200) NOT NULL,
        Roll_Number INT NOT NULL,
        InsertedAt DATETIME2(0) NOT NULL
            CONSTRAINT DF_tstUser_Audit_InsertedAt DEFAULT SYSUTCDATETIME()
    );
END;
GO

IF OBJECT_ID('dbo.trg_tstUser_AfterInsert', 'TR') IS NOT NULL
    DROP TRIGGER dbo.trg_tstUser_AfterInsert;
GO

CREATE TRIGGER dbo.trg_tstUser_AfterInsert
ON dbo.tstUser
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.tstUser_Audit (Id, Name, Roll_Number)
    SELECT Id, Name, Roll_Number
    FROM inserted;
END;
GO
