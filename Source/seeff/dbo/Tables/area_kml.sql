CREATE TABLE [dbo].[area_kml] (
    [kml_area_id] INT              IDENTITY (1, 1) NOT NULL,
    [area_id]     INT              NOT NULL,
    [latitude]    DECIMAL (18, 10) NOT NULL,
    [longitude]   DECIMAL (18, 10) NOT NULL,
    [seq]         INT              NOT NULL,
    PRIMARY KEY CLUSTERED ([kml_area_id] ASC)
);

