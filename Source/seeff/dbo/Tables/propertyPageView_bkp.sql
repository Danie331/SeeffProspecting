CREATE TABLE [dbo].[propertyPageView_bkp] (
    [propertyPageView_bkp_id] INT      IDENTITY (1, 1) NOT NULL,
    [propertyPageViewId]      INT      NOT NULL,
    [fkPropertyId]            INT      NULL,
    [fkBranchId]              INT      NULL,
    [fkAreaId]                INT      NULL,
    [fkProvinceId]            INT      NULL,
    [fkCountryId]             INT      NULL,
    [propertyPageViewCount]   INT      NULL,
    [propertyPageViewDate]    DATETIME NULL,
    CONSTRAINT [PK_propertyPageView_bkp] PRIMARY KEY CLUSTERED ([propertyPageView_bkp_id] ASC) WITH (FILLFACTOR = 90)
);

