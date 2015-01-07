CREATE TABLE [dbo].[area_layer] (
    [area_layer_id]         INT           IDENTITY (1, 1) NOT NULL,
    [area_id]               INT           NOT NULL,
    [area_type]             VARCHAR (1)   NOT NULL,
    [province_id]           INT           NULL,
    [formatted_poly_coords] VARCHAR (MAX) NOT NULL,
    PRIMARY KEY CLUSTERED ([area_layer_id] ASC)
);

