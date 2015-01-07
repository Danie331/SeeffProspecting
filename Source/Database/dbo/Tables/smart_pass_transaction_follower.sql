CREATE TABLE [dbo].[smart_pass_transaction_follower] (
    [smart_pass_id]          INT      NOT NULL,
    [smart_pass_follower_id] INT      NOT NULL,
    [created_by]             INT      NOT NULL,
    [created_date]           DATETIME NOT NULL
);

