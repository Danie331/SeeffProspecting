CREATE TABLE [dbo].[property_show_day] (
    [show_day_id]           BIGINT         IDENTITY (1, 1) NOT NULL,
    [fk_property_reference] INT            NOT NULL,
    [start_time]            DATETIME       NOT NULL,
    [end_time]              DATETIME       NULL,
    [directions]            VARCHAR (2000) NULL,
    [instructions]          VARCHAR (2000) NULL,
    [created_date]          DATETIME       DEFAULT (getdate()) NOT NULL,
    PRIMARY KEY CLUSTERED ([show_day_id] ASC)
);

