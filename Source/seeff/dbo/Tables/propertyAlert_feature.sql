CREATE TABLE [dbo].[propertyAlert_feature] (
    [propertyAlert_featureId]    NUMERIC (18)   IDENTITY (1, 1) NOT NULL,
    [fkPropertyAlertId]          NUMERIC (18)   NULL,
    [fkfeatureId]                INT            NULL,
    [propertyAlert_featureValue] NVARCHAR (200) NULL,
    CONSTRAINT [PK_propertyAlert_attribute] PRIMARY KEY CLUSTERED ([propertyAlert_featureId] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_propertyAlert_attribute_propertyAlert] FOREIGN KEY ([fkPropertyAlertId]) REFERENCES [dbo].[propertyAlert-old] ([propertyAlertId])
);

