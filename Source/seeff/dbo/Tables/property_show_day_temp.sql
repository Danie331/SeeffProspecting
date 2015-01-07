CREATE TABLE [dbo].[property_show_day_temp] (
    [show_day_id]           BIGINT         IDENTITY (1, 1) NOT NULL,
    [fk_property_reference] INT            NOT NULL,
    [show_time]             VARCHAR (200)  NOT NULL,
    [directions]            VARCHAR (2000) NULL,
    [created_date]          DATETIME       DEFAULT (getdate()) NOT NULL,
    PRIMARY KEY CLUSTERED ([show_day_id] ASC)
);

