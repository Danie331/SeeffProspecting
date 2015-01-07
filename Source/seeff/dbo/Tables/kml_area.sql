CREATE TABLE [dbo].[kml_area] (
    [area_id]   INT              NOT NULL,
    [latitude]  DECIMAL (18, 10) NOT NULL,
    [longitude] DECIMAL (18, 10) NOT NULL,
    [area_type] CHAR (1)         NOT NULL,
    [seq]       INT              NOT NULL,
    [kml_id]    INT              IDENTITY (1, 1) NOT NULL,
    PRIMARY KEY CLUSTERED ([kml_id] ASC)
);

