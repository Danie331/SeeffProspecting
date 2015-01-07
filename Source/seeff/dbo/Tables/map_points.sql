CREATE TABLE [dbo].[map_points] (
    [pk_map_point_id]     INT          IDENTITY (1, 1) NOT NULL,
    [map_point_name]      VARCHAR (50) NOT NULL,
    [map_point_permalink] VARCHAR (50) NULL,
    [map_point_filename]  VARCHAR (50) NULL,
    [map_point_interest]  TINYINT      CONSTRAINT [DF_map_points_map_point_interest] DEFAULT (0) NOT NULL,
    [map_point_active]    TINYINT      CONSTRAINT [DF_map_points_map_point_active] DEFAULT (0) NOT NULL,
    CONSTRAINT [PK_map_points] PRIMARY KEY CLUSTERED ([pk_map_point_id] ASC)
);

