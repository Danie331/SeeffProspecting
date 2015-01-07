CREATE TABLE [dbo].[property_images] (
    [property_imagesId]       NUMERIC (18)   IDENTITY (1, 1) NOT NULL,
    [property_imagesName]     NVARCHAR (100) NULL,
    [property_imagesFeatured] BIT            CONSTRAINT [DF_property_images_property_imagesFeatured] DEFAULT (0) NULL,
    [property_imagesDefault]  BIT            CONSTRAINT [DF_property_images_property_imagesDefault] DEFAULT (0) NULL,
    [propertyReference]       NUMERIC (18)   NULL,
    CONSTRAINT [PK_property_images] PRIMARY KEY CLUSTERED ([property_imagesId] ASC) WITH (FILLFACTOR = 90)
);


GO
CREATE NONCLUSTERED INDEX [propertyReference]
    ON [dbo].[property_images]([propertyReference] ASC) WITH (FILLFACTOR = 90);

