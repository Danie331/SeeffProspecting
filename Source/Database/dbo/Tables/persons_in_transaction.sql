CREATE TABLE [dbo].[persons_in_transaction] (
    [transaction_guid] VARCHAR (50)  NOT NULL,
    [person_id]        BIGINT        NOT NULL,
    [person_roll]      VARCHAR (150) NOT NULL
);

