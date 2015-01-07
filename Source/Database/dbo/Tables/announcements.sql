CREATE TABLE [dbo].[announcements] (
    [announcements_id]   INT            IDENTITY (1, 1) NOT NULL,
    [heading]            VARCHAR (50)   NOT NULL,
    [introduction]       VARCHAR (500)  NULL,
    [body]               NVARCHAR (MAX) NOT NULL,
    [created_date]       DATETIME       NOT NULL,
    [in_clause]          VARCHAR (250)  NOT NULL,
    [region]             VARCHAR (MAX)  NULL,
    [sub_region]         VARCHAR (MAX)  NULL,
    [license]            VARCHAR (MAX)  NULL,
    [created_by]         INT            NOT NULL,
    [announcements_guid] VARCHAR (50)   NULL,
    [urgent]             BIT            CONSTRAINT [DF_announcements_urgent] DEFAULT ((0)) NOT NULL,
    [poll_type]          VARCHAR (MAX)  CONSTRAINT [DF_announcements_poll_type] DEFAULT ('None') NOT NULL,
    [poll_answers]       VARCHAR (MAX)  CONSTRAINT [DF_announcements_poll_answers] DEFAULT ('None') NOT NULL
);

