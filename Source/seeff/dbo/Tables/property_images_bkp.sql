CREATE TABLE [dbo].[property_images_bkp] (
    [property_images_bkp_id]  INT            IDENTITY (1, 1) NOT NULL,
    [property_imagesId]       NUMERIC (18)   NOT NULL,
    [property_imagesName]     NVARCHAR (100) NULL,
    [property_imagesFeatured] BIT            CONSTRAINT [DF_property_images_bkp_property_imagesFeatured] DEFAULT (0) NULL,
    [property_imagesDefault]  BIT            CONSTRAINT [DF_property_images_bkp_property_imagesDefault] DEFAULT (0) NULL,
    [propertyReference]       NUMERIC (18)   NULL,
    CONSTRAINT [PK_property_images_bkp] PRIMARY KEY CLUSTERED ([property_images_bkp_id] ASC) WITH (FILLFACTOR = 90)
);

