CREATE TABLE [dbo].[searchBoxExc] (
    [pk_searchBoxExc_id]             INT           IDENTITY (1, 1) NOT NULL,
    [searchBoxExc_areaId]            INT           NULL,
    [searchBoxExc_areaName]          VARCHAR (255) NULL,
    [searchBoxExc_newSearchAreaName] VARCHAR (255) NULL,
    [searchBoxExc_addedArea]         VARCHAR (255) NULL,
    [searchBoxExc_added]             DATETIME      NULL,
    [searchBoxExc_updated]           DATETIME      NULL,
    [searchBoxExc_deleted]           TINYINT       CONSTRAINT [DF_searchBoxExc_searchBoxExc_deleted] DEFAULT (0) NULL
);

