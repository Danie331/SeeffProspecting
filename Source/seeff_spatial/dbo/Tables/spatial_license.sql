CREATE TABLE [dbo].[spatial_license] (
    [id]            INT               IDENTITY (1, 1) NOT NULL,
    [fk_license_id] INT               NULL,
    [geo_polygon]   [sys].[geography] NULL,
    [region]        VARCHAR (250)     NULL,
    [kml_coords]    NVARCHAR (MAX)    NULL
);

