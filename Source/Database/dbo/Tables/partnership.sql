CREATE TABLE [dbo].[partnership] (
    [partnership_id]   INT           IDENTITY (1, 1) NOT NULL,
    [branch_id]        INT           NOT NULL,
    [license_id]       INT           NOT NULL,
    [partner_count]    INT           NOT NULL,
    [partnership_name] VARCHAR (250) NOT NULL,
    [start_date]       DATETIME      NOT NULL,
    [end_date]         DATETIME      NULL,
    [section]          VARCHAR (50)  NOT NULL,
    [division]         VARCHAR (50)  NOT NULL,
    [partnership_guid] VARCHAR (50)  NOT NULL,
    [created_by]       INT           NOT NULL,
    [created_date]     DATETIME      CONSTRAINT [DF_partnership_created_date] DEFAULT (getdate()) NOT NULL
);

