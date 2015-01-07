CREATE TABLE [dbo].[branch_area] (
    [branch_areaId] INT IDENTITY (1, 1) NOT NULL,
    [fkAreaId]      INT NULL,
    [fkBranchId]    INT NULL,
    CONSTRAINT [PK_branch_area] PRIMARY KEY CLUSTERED ([branch_areaId] ASC) WITH (FILLFACTOR = 90)
);


GO
CREATE NONCLUSTERED INDEX [foreignKeys]
    ON [dbo].[branch_area]([fkAreaId] ASC, [fkBranchId] ASC) WITH (FILLFACTOR = 90);

