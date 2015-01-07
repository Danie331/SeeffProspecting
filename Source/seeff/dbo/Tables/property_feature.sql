CREATE TABLE [dbo].[property_feature] (
    [property_featureId]    NUMERIC (18)   IDENTITY (1, 1) NOT NULL,
    [fkPropertyId]          NUMERIC (18)   NULL,
    [fkfeatureId]           INT            NULL,
    [property_featureValue] NVARCHAR (200) NULL,
    CONSTRAINT [PK_property_attribute] PRIMARY KEY CLUSTERED ([property_featureId] ASC) WITH (FILLFACTOR = 90)
);


GO
CREATE NONCLUSTERED INDEX [foreignKeys]
    ON [dbo].[property_feature]([fkPropertyId] ASC, [fkfeatureId] ASC) WITH (FILLFACTOR = 90);

