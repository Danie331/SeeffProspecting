CREATE TABLE [dbo].[sps_hub_invoice_schedule] (
    [invoice_schedule_id]         INT             IDENTITY (1, 1) NOT NULL,
    [sps_transaction_ref]         VARCHAR (50)    NULL,
    [account_no]                  VARCHAR (50)    NOT NULL,
    [invoice_date]                DATETIME        NULL,
    [invoice_no]                  VARCHAR (50)    NULL,
    [invoice_sent]                BIT             CONSTRAINT [DF_sps_hub_invoice_schedule_invoice_sent] DEFAULT ((0)) NOT NULL,
    [branch_amt_ex_vat]           DECIMAL (18, 2) NULL,
    [sps_amt_ex_vat]              DECIMAL (18, 2) NULL,
    [total_incl_vat]              DECIMAL (18, 2) NULL,
    [money_received]              BIT             CONSTRAINT [DF_sps_hub_invoice_schedule_money_received] DEFAULT ((0)) NOT NULL,
    [seeff_deal]                  BIT             CONSTRAINT [DF_sps_hub_invoice_schedule_seeff] DEFAULT ((1)) NOT NULL,
    [created_date]                DATETIME        CONSTRAINT [DF_sps_hub_invoice_schedule_created_date] DEFAULT (getdate()) NOT NULL,
    [lightstone_reg_date]         VARCHAR (50)    NULL,
    [comm_paid_to_branch]         BIT             NULL,
    [sps_debt_remedy_ex_vat]      DECIMAL (18, 2) NULL,
    [national_debt_remedy_ex_vat] DECIMAL (18, 2) NULL,
    [invoice_folder]              VARCHAR (500)   NULL
);

