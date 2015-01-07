CREATE TABLE [dbo].[property_stadium] (
    [property_stadiumId]        NUMERIC (18) IDENTITY (1, 1) NOT NULL,
    [fk_stadium_2010_id]        INT          NULL,
    [fkPropertyId]              NUMERIC (18) NULL,
    [property_stadium_distance] INT          CONSTRAINT [DF_property_stadium_property_stadium_distance] DEFAULT (0) NULL,
    CONSTRAINT [PK_property_stadium] PRIMARY KEY CLUSTERED ([property_stadiumId] ASC)
);

