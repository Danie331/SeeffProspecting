CREATE TABLE [dbo].[property] (
    [propertyId]                        NUMERIC (18)    IDENTITY (1, 1) NOT NULL,
    [propertyReference]                 NUMERIC (18)    NOT NULL,
    [propertyReferenceOld]              NUMERIC (18)    NULL,
    [fkAgentId]                         INT             CONSTRAINT [DF_property_fkAgentId] DEFAULT (0) NULL,
    [fkBranchId]                        INT             NULL,
    [fkAreaId]                          INT             NULL,
    [fkCategoryId]                      INT             NOT NULL,
    [fkPropertyTypeId]                  INT             NULL,
    [fkActionId]                        INT             NULL,
    [propertyPrice]                     NUMERIC (19, 4) CONSTRAINT [DF_property_propertyPrice] DEFAULT (0) NOT NULL,
    [fkCurrencyId]                      INT             NULL,
    [fkGolfEstateId]                    INT             NULL,
    [fkCity2010Id]                      VARCHAR (50)    NULL,
    [fkStadium2010Id]                   VARCHAR (50)    NULL,
    [propertyShortDescription]          TEXT            NULL,
    [propertyFullDescription]           TEXT            NULL,
    [propertyBuildingSize]              INT             NULL,
    [propertyBuildingUnit]              INT             CONSTRAINT [DF_property_propertyBuildingUnit] DEFAULT (0) NULL,
    [propertyERFNumber]                 NVARCHAR (50)   NULL,
    [propertyERFSize]                   NVARCHAR (50)   NULL,
    [propertyERFUnit]                   INT             CONSTRAINT [DF_property_propertyERFUnit] DEFAULT (0) NULL,
    [propertyPriceNeg]                  BIT             NULL,
    [propertyRatesAndTaxes]             MONEY           CONSTRAINT [DF_property_propertyratesAndTaxes] DEFAULT (0) NOT NULL,
    [propertyRentalTerm]                CHAR (20)       CONSTRAINT [DF_property_propertyRentalTerm] DEFAULT ('n/a') NULL,
    [propertyMandate]                   CHAR (20)       CONSTRAINT [DF_property_propertyMandate] DEFAULT ('n/a') NULL,
    [propertySellerName]                NVARCHAR (50)   NULL,
    [propertyOccupationDate]            DATETIME        NULL,
    [propertyActive]                    BIT             CONSTRAINT [DF_property_propertyActive] DEFAULT (1) NOT NULL,
    [propertyVirtualTour]               NVARCHAR (20)   NULL,
    [propertyVirtualTourURL]            NVARCHAR (500)  NULL,
    [propertyVirtualTourX]              NVARCHAR (5)    NULL,
    [propertyVirtualTourY]              NVARCHAR (5)    NULL,
    [propertyVirtualTourGalleryURL]     NVARCHAR (255)  CONSTRAINT [DF_property_propertyVirtualTourOffsiteURL] DEFAULT ('') NULL,
    [propertyVirtualTourGalleryURLText] NVARCHAR (255)  CONSTRAINT [DF_property_propertyVirtualTourDescription] DEFAULT ('') NULL,
    [propertyCreated]                   DATETIME        CONSTRAINT [DF_property_propertyCreated] DEFAULT (getdate()) NULL,
    [propertyLastUpdated]               DATETIME        NULL,
    [propertyDateSold]                  DATETIME        NULL,
    [propertySellingPrice]              MONEY           CONSTRAINT [DF_property_propertySellingPrice] DEFAULT (0) NOT NULL,
    [fkSellerId]                        INT             CONSTRAINT [DF_property_fkCompetitorId] DEFAULT (0) NOT NULL,
    [propertyAddress]                   NVARCHAR (500)  NULL,
    [propertyTelephoneAreaCode]         NVARCHAR (50)   NULL,
    [propertyTelephone]                 NVARCHAR (50)   NULL,
    [propertyAdditionalFeatures]        NVARCHAR (3000) NULL,
    [propertyOnShow]                    BIT             CONSTRAINT [DF_property_propertyActive1] DEFAULT (0) NOT NULL,
    [propertyOnShowDirections]          NVARCHAR (2000) NULL,
    [propertyOnShowTime]                NVARCHAR (50)   NULL,
    [propertyFeatured]                  BIT             CONSTRAINT [DF_property_propertyFeatured] DEFAULT (0) NOT NULL,
    [propertySeeffSelect]               BIT             CONSTRAINT [DF_property_propertySeeffSelect] DEFAULT (0) NULL,
    [propertLastEditedBy]               INT             CONSTRAINT [DF_property_propertLastEditedBy] DEFAULT (0) NOT NULL,
    [propertyVetted]                    BIT             CONSTRAINT [DF_property_vetted] DEFAULT (0) NULL,
    [propertyGenieSyndicated]           TINYINT         CONSTRAINT [DF_property_propertyGenieSyndicated] DEFAULT (0) NULL,
    [propertyGenieExpiryDate]           DATETIME        NULL,
    [propertyGenieReference]            NVARCHAR (50)   NULL,
    [propertyGenieSuccessDate]          DATETIME        NULL,
    [propertyFnbQuicksell]              BIT             CONSTRAINT [DF_property_propertyFnbQuicksell] DEFAULT (0) NULL,
    [property2010AccommodationRental]   BIT             CONSTRAINT [DF_property_property2010AccommodationRental] DEFAULT (0) NULL,
    [propertyPOA]                       BIT             NULL,
    [fkRentalAdminId]                   INT             NULL,
    [fkMinRentalPeriodId]               NUMERIC (18)    NULL,
    [propertySellerEmail]               NVARCHAR (80)   NULL,
    [propertySleeps]                    INT             NULL,
    [propertyContactPerson]             NVARCHAR (80)   NULL,
    [propertyContactTelephone]          NVARCHAR (50)   NULL,
    [propertyContactEmail]              NVARCHAR (80)   NULL,
    [propertyIsPPPN]                    BIT             CONSTRAINT [DF_property_propertyIsPPPN_1] DEFAULT (0) NULL,
    [propertyBookingLink]               NVARCHAR (250)  NULL,
    [propertyBookingLinkText]           NVARCHAR (250)  NULL,
    [propertyBankRefNo]                 NVARCHAR (50)   NULL,
    [propertyUnitNumber]                NVARCHAR (50)   NULL,
    [propertyComplexName]               NVARCHAR (50)   NULL,
    [propertyStreetNumber]              NVARCHAR (5)    NULL,
    [propertyStreetName]                NVARCHAR (150)  NULL,
    [propertySuburb]                    NVARCHAR (50)   NULL,
    [propertyTown]                      NVARCHAR (50)   NULL,
    [propertyProvince]                  NVARCHAR (50)   NULL,
    [propertyPostalCode]                NVARCHAR (10)   NULL,
    [propertyCountry]                   NVARCHAR (50)   NULL,
    [propertyESPServiceProvider]        INT             CONSTRAINT [DF_property_propertyESPServiceProvider] DEFAULT (0) NULL,
    [propCntrolMandateId]               INT             NULL,
    [propCtrl_lastEditedName]           VARCHAR (255)   NULL,
    [propCtrl_lastEditedSurname]        VARCHAR (255)   NULL,
    [propCtrl_lastEditedUserId]         INT             NULL,
    [propCtrl_lastEditedEmail]          VARCHAR (255)   NULL,
    [propCtrl_lastEditedNumber]         VARCHAR (255)   NULL,
    [propertyLongitude]                 VARCHAR (255)   NULL,
    [propertyLicensee]                  VARCHAR (255)   NULL,
    [propertyLatitude]                  VARCHAR (255)   NULL,
    [propertyEdit]                      VARCHAR (255)   NULL,
    [petsAllowed]                       BIT             DEFAULT ((0)) NOT NULL,
    [furnished]                         BIT             DEFAULT ((0)) NOT NULL,
    [salePending]                       BIT             DEFAULT ((0)) NOT NULL,
    [expiryDate]                        DATETIME        NULL,
    [privatePropertyReference]          VARCHAR (100)   NULL,
    [privatePropertyCreatedDate]        DATETIME        NULL,
    [showAddress]                       BIT             DEFAULT ((0)) NOT NULL,
    [farmName]                          VARCHAR (150)   NULL,
    [videoLink]                         VARCHAR (500)   NULL,
    [privatePropertyStatus]             VARCHAR (1)     NULL,
    CONSTRAINT [PK_property] PRIMARY KEY CLUSTERED ([propertyId] ASC) WITH (FILLFACTOR = 90)
);


GO
CREATE NONCLUSTERED INDEX [foreignKeys]
    ON [dbo].[property]([fkAgentId] ASC, [fkBranchId] ASC, [fkAreaId] ASC, [fkCategoryId] ASC, [fkPropertyTypeId] ASC, [fkActionId] ASC, [fkCurrencyId] ASC, [fkGolfEstateId] ASC, [fkCity2010Id] ASC, [fkStadium2010Id] ASC, [fkSellerId] ASC) WITH (FILLFACTOR = 90);


GO
CREATE NONCLUSTERED INDEX [propertyReference]
    ON [dbo].[property]([propertyReference] ASC) WITH (FILLFACTOR = 90);


GO
CREATE NONCLUSTERED INDEX [toggleFlags]
    ON [dbo].[property]([propertyPriceNeg] ASC, [propertyActive] ASC, [propertyOnShow] ASC, [propertyFeatured] ASC, [propertySeeffSelect] ASC, [propertyVetted] ASC, [propertyGenieSyndicated] ASC, [propertyFnbQuicksell] ASC, [property2010AccommodationRental] ASC) WITH (FILLFACTOR = 90);


GO
-- =============================================
-- Author:		Adam Roberts
-- Create date: 2010 04 06 
-- Description:	Property Genie Delete
-- =============================================
CREATE TRIGGER [dbo].[pg_property_delete] 
   ON  dbo.property 
   AFTER DELETE
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	INSERT INTO [seeff].[dbo].[wl_pg_property_delete]
           ([propertyId]
           ,[propertyReference]
           ,[propertyGenieSyndicated]
           ,[propertyGenieExpiryDate]
           ,[propertyGenieReference]
           ,[propertyGenieSuccessDate]
           ,[propertyGenieDeleteDate])
     SELECT [propertyId]
	       ,[propertyReference]
	       ,[propertyGenieSyndicated]
	       ,[propertyGenieExpiryDate]
	       ,[propertyGenieReference]
	       ,[propertyGenieSuccessDate]
	       ,GETDATE()
       FROM Deleted d
  	  WHERE (propertyGenieReference IS NOT NULL
        AND propertyGenieReference <> '')
END
