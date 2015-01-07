CREATE TABLE [dbo].[property_extraFeature] (
    [property_extraFeatureId]    NUMERIC (18)    IDENTITY (1, 1) NOT NULL,
    [property_extraFeatureName]  NVARCHAR (200)  NULL,
    [property_extraFeatureValue] NVARCHAR (3000) NULL,
    [fkPropertyId]               NUMERIC (18)    NULL,
    CONSTRAINT [PK_property_extraFeature] PRIMARY KEY CLUSTERED ([property_extraFeatureId] ASC) WITH (FILLFACTOR = 90)
);

