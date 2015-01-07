CREATE TABLE [dbo].[prospecting_area_layer] (
    [area_layer_id]         INT           IDENTITY (1, 1) NOT NULL,
    [prospecting_area_id]   INT           NOT NULL,
    [area_type]             VARCHAR (1)   NOT NULL,
    [province_id]           INT           NULL,
    [formatted_poly_coords] VARCHAR (MAX) NOT NULL,
    PRIMARY KEY CLUSTERED ([area_layer_id] ASC),
    FOREIGN KEY ([prospecting_area_id]) REFERENCES [dbo].[prospecting_area] ([prospecting_area_id])
);

