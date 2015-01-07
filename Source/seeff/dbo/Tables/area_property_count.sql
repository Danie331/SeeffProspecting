CREATE TABLE [dbo].[area_property_count] (
    [area_property_count_id] INT           IDENTITY (1, 1) NOT NULL,
    [fkAreaId]               INT           NOT NULL,
    [areaName]               VARCHAR (255) NOT NULL,
    [fkAreaTypeId]           INT           NOT NULL,
    [fkParentAreaId]         INT           NULL,
    [buy_count]              BIGINT        NULL,
    [rent_count]             BIGINT        NULL,
    [updated_date]           DATETIME      DEFAULT (getdate()) NOT NULL,
    PRIMARY KEY CLUSTERED ([area_property_count_id] ASC)
);

