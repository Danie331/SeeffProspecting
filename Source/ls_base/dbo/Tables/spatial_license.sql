CREATE TABLE [dbo].[spatial_license] (
    [id]                INT               IDENTITY (1, 1) NOT NULL,
    [fk_license_id]     INT               NULL,
    [geo_polygon]       [sys].[geography] NULL,
    [region]            VARCHAR (100)     NULL,
    [kml_coords]        NVARCHAR (MAX)    NULL,
    [fk_territory_id]   INT               NULL,
    [area_center_point] AS                ([geo_polygon].[EnvelopeCenter]())
);

