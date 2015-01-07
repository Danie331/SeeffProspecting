CREATE TABLE [dbo].[prospecting_kml_area] (
    [prospecting_kml_area_id] INT              IDENTITY (1, 1) NOT NULL,
    [prospecting_area_id]     INT              NOT NULL,
    [latitude]                DECIMAL (18, 10) NOT NULL,
    [longitude]               DECIMAL (18, 10) NOT NULL,
    [area_type]               CHAR (1)         NOT NULL,
    [seq]                     INT              NOT NULL,
    PRIMARY KEY CLUSTERED ([prospecting_kml_area_id] ASC),
    CONSTRAINT [fk_prospecting_area_id] FOREIGN KEY ([prospecting_area_id]) REFERENCES [dbo].[prospecting_area] ([prospecting_area_id])
);

