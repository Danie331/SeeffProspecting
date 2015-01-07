CREATE TABLE [dbo].[kml_license] (
    [kml_license_id] INT              IDENTITY (1, 1) NOT NULL,
    [license_id]     INT              NOT NULL,
    [latitude]       DECIMAL (18, 10) NOT NULL,
    [longitude]      DECIMAL (18, 10) NOT NULL,
    [seq]            INT              NOT NULL,
    CONSTRAINT [PK_kml_license] PRIMARY KEY CLUSTERED ([kml_license_id] ASC)
);

