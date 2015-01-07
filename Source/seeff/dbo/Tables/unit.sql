CREATE TABLE [dbo].[unit] (
    [unitId]   INT           IDENTITY (1, 1) NOT NULL,
    [unitName] NVARCHAR (50) NULL,
    CONSTRAINT [PK_unit] PRIMARY KEY CLUSTERED ([unitId] ASC) WITH (FILLFACTOR = 90)
);

