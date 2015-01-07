CREATE TABLE [dbo].[map_property] (
    [pk_map_property_id]              INT           IDENTITY (1, 1) NOT NULL,
    [fk_branch_id]                    INT           NOT NULL,
    [fk_map_point_id]                 INT           NOT NULL,
    [fk_property_id]                  INT           NULL,
    [fk_property_id_apartment_parent] INT           NULL,
    [map_property_name]               VARCHAR (50)  NULL,
    [map_property_latitude]           VARCHAR (50)  NOT NULL,
    [map_property_longitude]          VARCHAR (50)  NOT NULL,
    [map_property_address]            VARCHAR (255) NULL,
    [map_property_added]              DATETIME      NULL,
    [map_property_updated]            DATETIME      NULL,
    CONSTRAINT [PK_map_property] PRIMARY KEY CLUSTERED ([pk_map_property_id] ASC)
);

