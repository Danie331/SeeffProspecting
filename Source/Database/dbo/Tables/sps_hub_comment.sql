CREATE TABLE [dbo].[sps_hub_comment] (
    [hub_comment_id]      BIGINT        IDENTITY (1, 1) NOT NULL,
    [sps_transaction_ref] VARCHAR (50)  NOT NULL,
    [registration_id]     BIGINT        NOT NULL,
    [subject]             VARCHAR (150) NOT NULL,
    [comment]             VARCHAR (MAX) NOT NULL,
    [created_date]        DATETIME      NOT NULL
);

