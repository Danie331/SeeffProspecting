CREATE TABLE [dbo].[propertyPrice] (
    [priceId]          INT           IDENTITY (1, 1) NOT NULL,
    [fkCountryId]      INT           CONSTRAINT [DF_propertyPrice_fkCountryId] DEFAULT ((-1)) NULL,
    [fkActionId]       INT           CONSTRAINT [DF_propertyPrice_fkActionId] DEFAULT ((-1)) NULL,
    [fkCategoryId]     INT           CONSTRAINT [DF_propertyPrice_fkCategoryId] DEFAULT ((-1)) NULL,
    [fkPropertyTypeId] INT           CONSTRAINT [DF_propertyPrice_fkPropertyTypeId] DEFAULT ((-1)) NULL,
    [priceDisplayed]   VARCHAR (255) NULL,
    [priceFrom]        NUMERIC (18)  NULL,
    [priceTo]          NUMERIC (18)  NULL,
    CONSTRAINT [PK_propertyPrice] PRIMARY KEY CLUSTERED ([priceId] ASC) WITH (FILLFACTOR = 90)
);

