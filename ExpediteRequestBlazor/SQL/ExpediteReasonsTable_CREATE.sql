USE [ExpediteRequest];
GO

CREATE TABLE dbo.ExpediteReasons (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Reason NVARCHAR(100) NOT NULL
);

INSERT INTO dbo.ExpediteReasons (Reason)
VALUES 
    (N'Line Down'),
    (N'Replacement Material (RMA)'),
    (N'Delivery Schedule Error'),
    (N'Unexpected Demand'),
    (N'Other (Enter in Comments)');