CREATE TABLE [dbo].[smart_pass_follower] (
    [smart_pass_follower_id] INT           IDENTITY (1, 1) NOT NULL,
    [follower_name]          VARCHAR (150) NOT NULL,
    [follower_surname]       VARCHAR (150) NOT NULL,
    [follower_email]         VARCHAR (150) NOT NULL,
    [created_by]             INT           NOT NULL,
    [created_date]           DATETIME      NOT NULL
);

