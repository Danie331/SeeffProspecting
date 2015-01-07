CREATE TABLE [dbo].[smart_pass_comment] (
    [smart_pass_comment_id] BIGINT        IDENTITY (1, 1) NOT NULL,
    [smart_pass_id]         INT           NOT NULL,
    [registration_id]       INT           NOT NULL,
    [subject]               VARCHAR (150) NOT NULL,
    [comment]               VARCHAR (MAX) NOT NULL,
    [created_date]          DATETIME      NOT NULL
);

