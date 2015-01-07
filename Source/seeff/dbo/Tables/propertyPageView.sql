CREATE TABLE [dbo].[propertyPageView] (
    [propertyPageViewId]    INT      IDENTITY (1, 1) NOT NULL,
    [fkPropertyId]          INT      NULL,
    [fkBranchId]            INT      NULL,
    [fkAreaId]              INT      NULL,
    [fkProvinceId]          INT      NULL,
    [fkCountryId]           INT      NULL,
    [propertyPageViewCount] INT      NULL,
    [propertyPageViewDate]  DATETIME NULL,
    CONSTRAINT [PK_propertyReport] PRIMARY KEY CLUSTERED ([propertyPageViewId] ASC) WITH (FILLFACTOR = 90)
);

