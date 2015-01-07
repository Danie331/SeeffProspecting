CREATE TABLE [dbo].[property_features] (
    [property_features_id]         INT             IDENTITY (1, 1) NOT NULL,
    [fk_property_reference]        INT             NOT NULL,
    [fk_property_features_type_id] INT             NOT NULL,
    [property_features_count]      DECIMAL (18, 2) DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([property_features_id] ASC)
);

