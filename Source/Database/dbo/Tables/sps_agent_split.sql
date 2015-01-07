CREATE TABLE [dbo].[sps_agent_split] (
    [sps_transaction_ref] VARCHAR (50)    NOT NULL,
    [registration_id]     BIGINT          NOT NULL,
    [comm_paid]           DECIMAL (18, 2) NOT NULL,
    [first_sale]          VARCHAR (3)     CONSTRAINT [DF_sps_agent_split_first_sale_1] DEFAULT ('No') NOT NULL,
    [recognise]           BIT             CONSTRAINT [DF_sps_agent_split_recognise] DEFAULT ((1)) NOT NULL
);


GO
CREATE NONCLUSTERED INDEX [Indx01]
    ON [dbo].[sps_agent_split]([sps_transaction_ref] ASC)
    INCLUDE([registration_id], [comm_paid], [recognise]);

