CREATE TABLE [dbo].[announcements_vote] (
    [announcements_id] INT            NOT NULL,
    [registration_id]  INT            NOT NULL,
    [choice]           NVARCHAR (MAX) NOT NULL,
    [created_date]     DATETIME       CONSTRAINT [DF_announcements_vote_created_date] DEFAULT (getdate()) NOT NULL
);

