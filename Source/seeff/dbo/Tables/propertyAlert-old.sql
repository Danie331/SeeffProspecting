CREATE TABLE [dbo].[propertyAlert-old] (
    [propertyAlertId]         NUMERIC (18)  IDENTITY (1, 1) NOT NULL,
    [fkWebUserId]             NUMERIC (18)  NULL,
    [AreaName]                NVARCHAR (50) NULL,
    [fkCurrencyId]            INT           NULL,
    [fkPriceId]               INT           NULL,
    [fkCateryoryId]           INT           NULL,
    [fkActionId]              INT           NULL,
    [PropertyType]            NVARCHAR (50) NULL,
    [FeatureType]             NVARCHAR (50) NULL,
    [propertyAlertInsertDate] SMALLDATETIME NULL,
    [propertyAlertLastAlert]  SMALLDATETIME NULL,
    CONSTRAINT [PK_propertyAlert] PRIMARY KEY CLUSTERED ([propertyAlertId] ASC) WITH (FILLFACTOR = 90)
);

