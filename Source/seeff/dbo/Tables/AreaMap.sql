CREATE TABLE [dbo].[AreaMap] (
    [areaMapId]      BIGINT          IDENTITY (1, 1) NOT NULL,
    [sPath]          NVARCHAR (2000) NULL,
    [fkAreaId]       INT             NULL,
    [fkAreaParentId] INT             NULL,
    [areaName]       VARCHAR (100)   NULL,
    [fkAreaTypeId]   INT             NULL,
    CONSTRAINT [PK_AreaMap] PRIMARY KEY CLUSTERED ([areaMapId] ASC) WITH (FILLFACTOR = 90)
);

