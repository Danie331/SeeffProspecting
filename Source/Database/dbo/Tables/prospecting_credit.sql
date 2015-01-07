CREATE TABLE [dbo].[prospecting_credit] (
    [prospecting_credits_id] INT             IDENTITY (1, 1) NOT NULL,
    [license_id]             INT             NOT NULL,
    [credits_loaded]         INT             NOT NULL,
    [credits_allocated]      INT             NOT NULL,
    [allocated_to]           INT             NOT NULL,
    [invoice_amount]         DECIMAL (18, 2) NULL,
    [vat_amount]             DECIMAL (18, 2) NULL,
    [created_date]           DATETIME        NOT NULL,
    [created_by]             INT             NOT NULL,
    CONSTRAINT [PK_prospecting_credit] PRIMARY KEY CLUSTERED ([prospecting_credits_id] ASC)
);

