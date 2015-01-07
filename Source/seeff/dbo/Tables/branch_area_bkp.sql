CREATE TABLE [dbo].[branch_area_bkp] (
    [branch_area_bkp_id] INT IDENTITY (1, 1) NOT NULL,
    [branch_areaId]      INT NOT NULL,
    [fkAreaId]           INT NULL,
    [fkBranchId]         INT NULL,
    CONSTRAINT [PK_branch_area_bkp] PRIMARY KEY CLUSTERED ([branch_area_bkp_id] ASC) WITH (FILLFACTOR = 90)
);

