CREATE TABLE [dbo].[spatial_terretory] (
    [territory_id]   INT               IDENTITY (1, 1) NOT NULL,
    [territory_name] VARCHAR (50)      NULL,
    [geo_polygon]    [sys].[geography] NULL
);

