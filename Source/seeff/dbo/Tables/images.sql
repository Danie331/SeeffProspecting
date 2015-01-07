CREATE TABLE [dbo].[images] (
    [imagesId]       NUMERIC (18)   IDENTITY (1, 1) NOT NULL,
    [fkPropertyId]   NUMERIC (18)   NULL,
    [imagesFilename] NVARCHAR (255) NULL,
    [imagesPrimary]  BIT            CONSTRAINT [DF_images_imagesPrimary] DEFAULT (0) NOT NULL,
    CONSTRAINT [PK_images] PRIMARY KEY CLUSTERED ([imagesId] ASC) WITH (FILLFACTOR = 90)
);


GO
CREATE NONCLUSTERED INDEX [foreignKeys]
    ON [dbo].[images]([fkPropertyId] ASC) WITH (FILLFACTOR = 90);

