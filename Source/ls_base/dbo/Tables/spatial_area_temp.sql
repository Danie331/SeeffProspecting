CREATE TABLE [dbo].[spatial_area_temp] (
    [fkAreaId]      INT               NULL,
    [geo_polygon]   [sys].[geography] NULL,
    [fk_license_id] INT               NULL,
    [kml_coords]    NVARCHAR (MAX)    NULL
);

