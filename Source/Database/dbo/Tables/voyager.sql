CREATE TABLE [dbo].[voyager] (
    [voyager_id]          INT             IDENTITY (1, 1) NOT NULL,
    [deal_no]             VARCHAR (100)   NOT NULL,
    [branch_id]           INT             NOT NULL,
    [loaded_by]           INT             NOT NULL,
    [loaded_date]         DATETIME        NOT NULL,
    [voyager_no]          INT             NOT NULL,
    [voyager_member_name] VARCHAR (250)   NOT NULL,
    [transaction_date]    DATETIME        NOT NULL,
    [voyager_points]      INT             NOT NULL,
    [miles_rand_value]    DECIMAL (18, 2) NOT NULL,
    [seller_buyer]        VARCHAR (6)     NOT NULL,
    [email_address]       VARCHAR (250)   NULL,
    [batch_no]            INT             NOT NULL,
    [processed]           BIT             NOT NULL,
    [processed_date]      DATETIME        NULL,
    [rejected]            BIT             NOT NULL,
    [rejected_reason]     VARCHAR (250)   NOT NULL
);

