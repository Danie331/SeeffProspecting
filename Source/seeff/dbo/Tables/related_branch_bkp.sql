CREATE TABLE [dbo].[related_branch_bkp] (
    [related_branch_bkp_id] INT IDENTITY (1, 1) NOT NULL,
    [related_branchId]      INT NOT NULL,
    [fkBranchId]            INT NULL,
    [fkRelatedBranchId]     INT NULL,
    CONSTRAINT [PK_related_branch_bkp] PRIMARY KEY CLUSTERED ([related_branch_bkp_id] ASC) WITH (FILLFACTOR = 90)
);

