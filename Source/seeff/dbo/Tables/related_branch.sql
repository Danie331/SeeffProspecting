CREATE TABLE [dbo].[related_branch] (
    [related_branchId]  INT IDENTITY (1, 1) NOT NULL,
    [fkBranchId]        INT NULL,
    [fkRelatedBranchId] INT NULL,
    CONSTRAINT [PK_related_branch] PRIMARY KEY CLUSTERED ([related_branchId] ASC)
);

