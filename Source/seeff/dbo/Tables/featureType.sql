CREATE TABLE [dbo].[featureType] (
    [featureTypeId]   INT           IDENTITY (1, 1) NOT NULL,
    [featureTypeName] NVARCHAR (50) NULL,
    CONSTRAINT [PK_attributeType] PRIMARY KEY CLUSTERED ([featureTypeId] ASC) WITH (FILLFACTOR = 90)
);

