CREATE TABLE [dbo].[propertyType] (
    [propertyTypeId]      INT           IDENTITY (1, 1) NOT NULL,
    [propertyTypeName]    NVARCHAR (50) NULL,
    [fkCategoryId]        INT           CONSTRAINT [DF_propertyType_fkCategoryId] DEFAULT (0) NOT NULL,
    [propertyTypeCanRent] BIT           CONSTRAINT [DF_propertyType_propertyTypeCanRent] DEFAULT (0) NULL,
    CONSTRAINT [PK_propertyType] PRIMARY KEY CLUSTERED ([propertyTypeId] ASC) WITH (FILLFACTOR = 90)
);

