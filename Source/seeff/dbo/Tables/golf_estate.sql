CREATE TABLE [dbo].[golf_estate] (
    [pk_golf_estate_id]        INT           IDENTITY (1, 1) NOT NULL,
    [fk_area_id]               INT           NULL,
    [golf_estate_title]        VARCHAR (150) NULL,
    [golf_estate_image]        VARCHAR (250) NULL,
    [golf_estate_short_intro]  TEXT          NULL,
    [golf_estate_date_added]   DATETIME      NULL,
    [golf_estate_date_updated] DATETIME      NULL,
    [golf_estate_active]       TINYINT       CONSTRAINT [DF_golf_estate_golf_estate_active] DEFAULT (0) NULL,
    [golf_estate_deleted]      TINYINT       CONSTRAINT [DF_golf_estate_golf_estate_deleted] DEFAULT (0) NULL,
    CONSTRAINT [PK_golf_estate] PRIMARY KEY CLUSTERED ([pk_golf_estate_id] ASC) WITH (FILLFACTOR = 90)
);


GO
CREATE NONCLUSTERED INDEX [foreignKeys]
    ON [dbo].[golf_estate]([fk_area_id] ASC) WITH (FILLFACTOR = 90);

