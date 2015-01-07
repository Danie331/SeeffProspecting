CREATE TABLE [dbo].[country] (
    [countryId]              INT           IDENTITY (1, 1) NOT NULL,
    [countryName]            NVARCHAR (50) NULL,
    [countryDialCode]        INT           NULL,
    [countryCode]            NVARCHAR (50) NULL,
    [currencyName]           NVARCHAR (50) NULL,
    [currencySymbol]         NVARCHAR (50) NULL,
    [currencyConversionRate] FLOAT (53)    CONSTRAINT [DF_country_currencyConversionRate] DEFAULT (0) NOT NULL,
    [currencyConvert]        BIT           CONSTRAINT [DF_country_currencyConvert] DEFAULT (0) NOT NULL,
    [currencyIsDefault]      BIT           CONSTRAINT [DF_country_currencyDefault] DEFAULT (0) NOT NULL,
    [currencyLoadProperty]   BIT           CONSTRAINT [DF_country_currencyLoadProperty] DEFAULT (0) NULL,
    [currencyDisplayOrder]   INT           CONSTRAINT [DF_country_currencyDisplayOrder] DEFAULT (999) NULL,
    CONSTRAINT [PK_country] PRIMARY KEY CLUSTERED ([countryId] ASC) WITH (FILLFACTOR = 90)
);

