CREATE TABLE [dbo].[live_images] (
    [pk_live_image_id]              INT          IDENTITY (1, 1) NOT NULL,
    [live_image_property_reference] NUMERIC (18) NOT NULL,
    CONSTRAINT [PK_live_images] PRIMARY KEY CLUSTERED ([pk_live_image_id] ASC) WITH (FILLFACTOR = 90)
);

