CREATE TABLE [dbo].[spatial_area] (
    [id]                INT               IDENTITY (1, 1) NOT NULL,
    [fkAreaId]          INT               NULL,
    [geo_polygon]       [sys].[geography] NULL,
    [fk_license_id]     INT               NULL,
    [kml_coords]        NVARCHAR (MAX)    NULL,
    [fk_territory_id]   INT               NULL,
    [area_center_point] AS                ([geo_polygon].[EnvelopeCenter]())
);

