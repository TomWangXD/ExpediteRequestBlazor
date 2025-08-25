-- Create ApproverStatus Lookup Table
CREATE TABLE [dbo].[ApproverStatusLookup] (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    StatusName NVARCHAR(100) NOT NULL UNIQUE
);

-- Insert dropdown values
INSERT INTO [dbo].[ApproverStatusLookup] (StatusName)
VALUES
    ('Approve Total'),
    ('Approve Partial'),
    ('Declined'),
    ('Date Modified');