CREATE TABLE [dbo].[neighbouring_area] (
    [neighbouring_areaId]  INT IDENTITY (1, 1) NOT NULL,
    [fkAreaId]             INT NULL,
    [fkNeighbouringAreaId] INT NULL,
    CONSTRAINT [PK_neighbouring_area] PRIMARY KEY CLUSTERED ([neighbouring_areaId] ASC) WITH (FILLFACTOR = 90)
);


GO
CREATE NONCLUSTERED INDEX [foreignKeys]
    ON [dbo].[neighbouring_area]([fkAreaId] ASC, [fkNeighbouringAreaId] ASC) WITH (FILLFACTOR = 90);

