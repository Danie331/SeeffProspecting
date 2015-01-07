CREATE TABLE [dbo].[prospecting_area] (
    [prospecting_area_id] INT            NOT NULL,
    [area_name]           NVARCHAR (100) NOT NULL,
    CONSTRAINT [pk_prospecting_area_id] PRIMARY KEY CLUSTERED ([prospecting_area_id] ASC)
);

