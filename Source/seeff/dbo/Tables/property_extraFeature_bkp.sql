CREATE TABLE [dbo].[property_extraFeature_bkp] (
    [property_extraFeature_bkp_id] INT             IDENTITY (1, 1) NOT NULL,
    [property_extraFeatureId]      NUMERIC (18)    NOT NULL,
    [property_extraFeatureName]    NVARCHAR (200)  NULL,
    [property_extraFeatureValue]   NVARCHAR (3000) NULL,
    [fkPropertyId]                 NUMERIC (18)    NULL,
    CONSTRAINT [PK_property_extraFeature_bkp] PRIMARY KEY CLUSTERED ([property_extraFeature_bkp_id] ASC) WITH (FILLFACTOR = 90)
);

