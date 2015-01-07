CREATE TABLE [dbo].[propertyAlert] (
    [propertyAlertId]         NUMERIC (18)   IDENTITY (1, 1) NOT NULL,
    [fkAreaId]                INT            NULL,
    [fkWebUserId]             NUMERIC (18)   NULL,
    [fkCurrencyId]            INT            NULL,
    [priceFrom]               INT            NULL,
    [priceTo]                 BIGINT         NULL,
    [fkActionId]              INT            NULL,
    [fkCateryoryId]           INT            NULL,
    [PropertyType]            NVARCHAR (50)  NULL,
    [FeatureType]             NVARCHAR (50)  NULL,
    [propertyAlertInsertDate] SMALLDATETIME  NULL,
    [propertyAlertLastAlert]  SMALLDATETIME  NULL,
    [propertyBranchId]        INT            NULL,
    [propertySuburbId]        INT            NULL,
    [propertyAlertFirstName]  NVARCHAR (100) NULL,
    [propertyAlertSurname]    NVARCHAR (100) NULL,
    [propertyAlertEmail]      NVARCHAR (100) NULL,
    [propertyAlertTelephone]  NVARCHAR (100) NULL,
    CONSTRAINT [PK_propertyAlert_1] PRIMARY KEY CLUSTERED ([propertyAlertId] ASC) WITH (FILLFACTOR = 90)
);

