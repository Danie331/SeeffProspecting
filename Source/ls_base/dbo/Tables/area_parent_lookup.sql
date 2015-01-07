CREATE TABLE [dbo].[area_parent_lookup] (
    [area_parent_lookup_id] INT           IDENTITY (1, 1) NOT NULL,
    [name]                  VARCHAR (MAX) NOT NULL,
    [area_id]               INT           NOT NULL,
    [area_type]             VARCHAR (8)   NOT NULL,
    PRIMARY KEY CLUSTERED ([area_parent_lookup_id] ASC)
);

