CREATE TABLE [dbo].[branch] (
    [branchId]   INT            NOT NULL,
    [branchName] NVARCHAR (100) NOT NULL,
    [active]     BIT            CONSTRAINT [DF_branch_active] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_branch] PRIMARY KEY CLUSTERED ([branchId] ASC) WITH (FILLFACTOR = 90)
);

