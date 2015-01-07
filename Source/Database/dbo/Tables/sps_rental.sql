CREATE TABLE [dbo].[sps_rental] (
    [sps_transaction_ref]           VARCHAR (50)    NOT NULL,
    [next_rental_increase_date]     DATETIME        NULL,
    [agent_admin_cost]              DECIMAL (18, 2) NULL,
    [vat]                           DECIMAL (18, 2) NULL,
    [bank_charges]                  DECIMAL (18, 2) NULL,
    [maintenance]                   DECIMAL (18, 2) NULL,
    [inspection_fees]               DECIMAL (18, 2) NULL,
    [monthly_commission_percentage] DECIMAL (18, 2) NULL
);

