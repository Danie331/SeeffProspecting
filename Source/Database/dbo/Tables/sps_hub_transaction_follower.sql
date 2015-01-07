CREATE TABLE [dbo].[sps_hub_transaction_follower] (
    [sps_transaction_ref] VARCHAR (50) NOT NULL,
    [hub_follower_id]     INT          NOT NULL,
    [created_by]          INT          NOT NULL,
    [created_date]        DATETIME     NOT NULL
);

