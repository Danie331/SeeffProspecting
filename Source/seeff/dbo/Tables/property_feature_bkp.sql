CREATE TABLE [dbo].[property_feature_bkp] (
    [property_feature_bkp_id] INT            IDENTITY (1, 1) NOT NULL,
    [property_featureId]      NUMERIC (18)   NOT NULL,
    [fkPropertyId]            NUMERIC (18)   NULL,
    [fkfeatureId]             INT            NULL,
    [property_featureValue]   NVARCHAR (200) NULL,
    CONSTRAINT [PK_property_feature_bkp] PRIMARY KEY CLUSTERED ([property_feature_bkp_id] ASC) WITH (FILLFACTOR = 90)
);

