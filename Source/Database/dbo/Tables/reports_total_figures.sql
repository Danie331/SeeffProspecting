CREATE TABLE [dbo].[reports_total_figures] (
    [sps_transaction_id]   BIGINT          NOT NULL,
    [comm_amount]          DECIMAL (18, 2) NOT NULL,
    [selling_price]        DECIMAL (18, 2) NOT NULL,
    [referral_comm]        DECIMAL (18, 2) NULL,
    [total_comm]           DECIMAL (18, 2) NOT NULL,
    [at_seven_prec]        DECIMAL (18, 2) NOT NULL,
    [comm_perc]            DECIMAL (18, 4) NOT NULL,
    [license_id]           INT             NOT NULL,
    [license_name]         VARCHAR (250)   NOT NULL,
    [reporting_date]       DATETIME        NULL,
    [transaction_division] VARCHAR (50)    NOT NULL
);

