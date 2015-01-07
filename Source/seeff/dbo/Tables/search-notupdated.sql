CREATE TABLE [dbo].[search-notupdated] (
    [searchId]                              NUMERIC (18)    IDENTITY (1, 1) NOT NULL,
    [searchCountry]                         NUMERIC (10)    NOT NULL,
    [searchProvince]                        NUMERIC (18)    NOT NULL,
    [searchRegion]                          NUMERIC (18)    NOT NULL,
    [searchCity]                            NUMERIC (18)    NOT NULL,
    [searchArea]                            NUMERIC (18)    NOT NULL,
    [searchSuburb]                          NUMERIC (18)    NOT NULL,
    [searchAreaName]                        NVARCHAR (200)  NULL,
    [searchAreaParentName]                  NVARCHAR (200)  NULL,
    [fkPropertyId]                          NUMERIC (18)    NOT NULL,
    [fkAgentId]                             INT             NULL,
    [fkBranchId]                            INT             NOT NULL,
    [fkAreaParentId]                        INT             NULL,
    [fkAreaId]                              INT             NOT NULL,
    [fkCategoryId]                          INT             NOT NULL,
    [fkPropertyTypeId]                      INT             NOT NULL,
    [searchPropertyTypeName]                NVARCHAR (200)  NULL,
    [fkActionId]                            INT             NOT NULL,
    [fkGolfEstateId]                        INT             NULL,
    [fkCity2010Id]                          VARCHAR (50)    NULL,
    [fkStadium2010Id]                       VARCHAR (50)    NULL,
    [searchReference]                       VARCHAR (20)    NULL,
    [searchPrice]                           NUMERIC (19, 4) NULL,
    [searchPriceAdjusted]                   NUMERIC (19, 4) NULL,
    [fkCurrencyId]                          INT             NULL,
    [searchShortDescription]                NVARCHAR (3000) NULL,
    [searchRentalTerm]                      CHAR (20)       NULL,
    [searchBuildingSize]                    INT             NULL,
    [searchBuildingUnit]                    INT             NULL,
    [searchBuildingPerm2]                   MONEY           NULL,
    [searchERFSize]                         INT             NULL,
    [searchERFUnit]                         INT             NULL,
    [searchLandPerm2]                       MONEY           NULL,
    [searchBedrooms]                        INT             NULL,
    [searchBathrooms]                       INT             NULL,
    [searchGarages]                         INT             NULL,
    [searchSecureParking]                   INT             NULL,
    [searchReceptions]                      INT             NULL,
    [searchImage]                           NVARCHAR (100)  NULL,
    [searchOnShow]                          BIT             CONSTRAINT [DF_search_searchOnShow] DEFAULT (0) NULL,
    [searchPropertySold]                    BIT             CONSTRAINT [DF_search_searchPropertySold] DEFAULT (0) NULL,
    [searchPropertySeeffSelect]             BIT             CONSTRAINT [DF_search_searchPropertySeeffSelect] DEFAULT (0) NULL,
    [searchPropertyFnbQuicksell]            BIT             CONSTRAINT [DF_search_searchPropertyFnbQuicksell] DEFAULT (0) NULL,
    [searchPropertyLastUpdated]             DATETIME        NULL,
    [searchAgentName]                       NVARCHAR (100)  NULL,
    [searchBranchName]                      NVARCHAR (100)  NULL,
    [searchPropertyGenieReference]          NVARCHAR (50)   NULL,
    [searchPropertyGenieDateSuccess]        DATETIME        NULL,
    [searchProperty2010AccommodationRental] BIT             CONSTRAINT [DF_search_searchProperty2010AccommodationRental] DEFAULT (0) NULL,
    [searchPropertyPOA]                     BIT             NULL,
    [searchPropertySleeps]                  INT             CONSTRAINT [DF_search_searchPropertySleeps] DEFAULT (0) NULL
);


GO
CREATE NONCLUSTERED INDEX [foreignKeysArea]
    ON [dbo].[search-notupdated]([searchCountry] ASC, [searchProvince] ASC, [searchRegion] ASC, [searchCity] ASC, [searchArea] ASC, [searchSuburb] ASC) WITH (FILLFACTOR = 90);


GO
CREATE NONCLUSTERED INDEX [proeprtyReference]
    ON [dbo].[search-notupdated]([searchReference] ASC) WITH (FILLFACTOR = 90);

