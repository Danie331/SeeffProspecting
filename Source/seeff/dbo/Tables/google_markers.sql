CREATE TABLE [dbo].[google_markers] (
    [pk_google_marker_id]     INT          IDENTITY (1, 1) NOT NULL,
    [fk_map_point_id]         INT          NULL,
    [fk_branch_id]            INT          NULL,
    [google_marker_name]      VARCHAR (50) NULL,
    [google_marker_address]   VARCHAR (50) NULL,
    [google_marker_image]     VARCHAR (50) NULL,
    [google_marker_longitude] VARCHAR (50) NULL,
    [google_marker_latitude]  VARCHAR (50) NULL,
    [google_marker_added]     DATETIME     NULL,
    [google_marker_updated]   DATETIME     NULL,
    [google_marker_active]    TINYINT      NULL,
    [google_marker_deleted]   TINYINT      NULL
);

