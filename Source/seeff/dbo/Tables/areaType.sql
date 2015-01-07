CREATE TABLE [dbo].[areaType] (
    [areaTypeId]   INT           IDENTITY (1, 1) NOT NULL,
    [areaTypeName] NVARCHAR (50) NULL,
    CONSTRAINT [PK_areaType] PRIMARY KEY CLUSTERED ([areaTypeId] ASC) WITH (FILLFACTOR = 90)
);

