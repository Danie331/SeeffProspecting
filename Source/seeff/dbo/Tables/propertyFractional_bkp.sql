CREATE TABLE [dbo].[propertyFractional_bkp] (
    [propertyFractional_bkp_id] INT IDENTITY (1, 1) NOT NULL,
    [propertyFractionalId]      INT NOT NULL,
    [fkPropertyId]              INT NULL,
    [fkFractionalId]            INT NULL,
    CONSTRAINT [PK_propertyFractional_bkp] PRIMARY KEY CLUSTERED ([propertyFractional_bkp_id] ASC) WITH (FILLFACTOR = 90)
);

