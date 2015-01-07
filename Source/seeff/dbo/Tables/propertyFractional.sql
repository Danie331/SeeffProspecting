CREATE TABLE [dbo].[propertyFractional] (
    [propertyFractionalId] INT IDENTITY (1, 1) NOT NULL,
    [fkPropertyId]         INT NULL,
    [fkFractionalId]       INT NULL,
    CONSTRAINT [PK_propertyFractional] PRIMARY KEY CLUSTERED ([propertyFractionalId] ASC) WITH (FILLFACTOR = 90)
);

