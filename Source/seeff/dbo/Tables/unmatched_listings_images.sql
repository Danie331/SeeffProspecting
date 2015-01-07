CREATE TABLE [dbo].[unmatched_listings_images] (
    [PK_unmatched_id]        INT           IDENTITY (1, 1) NOT NULL,
    [fk_propertyReference]   INT           NULL,
    [fk_property_imagesName] VARCHAR (100) NULL,
    [image_status]           VARCHAR (100) DEFAULT ('pending') NULL,
    PRIMARY KEY CLUSTERED ([PK_unmatched_id] ASC)
);

