CREATE TABLE [dbo].[branchType] (
    [branchTypeId]   INT           IDENTITY (1, 1) NOT NULL,
    [branchTypeName] NVARCHAR (50) NULL,
    CONSTRAINT [PK_branchType] PRIMARY KEY CLUSTERED ([branchTypeId] ASC) WITH (FILLFACTOR = 90)
);

