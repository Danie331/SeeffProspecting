CREATE TABLE [dbo].[area] (
    [areaId]                           INT             IDENTITY (1, 1) NOT NULL,
    [areaName]                         NVARCHAR (100)  NULL,
    [areaParentId]                     INT             NULL,
    [fkAreaTypeId]                     INT             NULL,
    [nDisplayOrder]                    INT             NULL,
    [areaMap]                          NVARCHAR (50)   NULL,
    [sideMap]                          NVARCHAR (50)   NULL,
    [areaImageMap]                     NVARCHAR (4000) NULL,
    [areaDescription]                  NVARCHAR (4000) NULL,
    [areaPropertyCount]                INT             CONSTRAINT [DF_area_areaPropertyCount] DEFAULT (0) NULL,
    [areaResidentialPropertyBuyCount]  INT             CONSTRAINT [DF_area_areaResidentialPropertyBuyCount] DEFAULT (0) NULL,
    [areaResidentialPropertyRentCount] INT             CONSTRAINT [DF_area_areaResidentialPropertyRentCount] DEFAULT (0) NULL,
    [areaCommercialPropertyBuyCount]   INT             CONSTRAINT [DF_area_areaCommercialPropertyBuyCount] DEFAULT (0) NULL,
    [areaCommercialPropertyRentCount]  INT             CONSTRAINT [DF_area_areaCommercialPropertyRentCount] DEFAULT (0) NULL,
    [areaDevelopementPropertyCount]    INT             CONSTRAINT [DF_area_areaDevelopementPropertyCount] DEFAULT (0) NULL,
    [areaAgriculturalPropertyCount]    INT             CONSTRAINT [DF_area_areaAgriculturalPropertyCount] DEFAULT (0) NULL,
    CONSTRAINT [PK_area] PRIMARY KEY CLUSTERED ([areaId] ASC) WITH (FILLFACTOR = 90)
);


GO
CREATE NONCLUSTERED INDEX [foreignKeys]
    ON [dbo].[area]([areaParentId] ASC, [fkAreaTypeId] ASC) WITH (FILLFACTOR = 90);

