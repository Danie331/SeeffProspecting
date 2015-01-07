CREATE TABLE [dbo].[related_area] (
    [related_areaId]  INT IDENTITY (1, 1) NOT NULL,
    [fkAreaId]        INT CONSTRAINT [DF_related_area_fkAreaId] DEFAULT (0) NOT NULL,
    [fkRelatedAreaId] INT CONSTRAINT [DF_related_area_fkRelatedAreaId] DEFAULT (0) NOT NULL,
    CONSTRAINT [PK_related_area] PRIMARY KEY CLUSTERED ([related_areaId] ASC) WITH (FILLFACTOR = 90)
);

