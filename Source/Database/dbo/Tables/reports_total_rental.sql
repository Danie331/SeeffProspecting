CREATE TABLE [dbo].[reports_total_rental] (
    [sps_transaction_id]   BIGINT          NOT NULL,
    [monthly_rental]       DECIMAL (18, 2) NOT NULL,
    [non_managed_comm]     DECIMAL (18, 2) NULL,
    [managed_comm]         DECIMAL (18, 2) NULL,
    [referral_comm]        DECIMAL (18, 2) NULL,
    [total_comm]           DECIMAL (18, 2) NOT NULL,
    [at_seven_prec]        DECIMAL (18, 2) NOT NULL,
    [comm_perc]            DECIMAL (18, 4) NOT NULL,
    [license_id]           INT             NOT NULL,
    [license_name]         VARCHAR (250)   NOT NULL,
    [renewal]              BIT             CONSTRAINT [DF_reports_total_rental_renewal] DEFAULT ((0)) NOT NULL,
    [schedule_payment]     BIT             CONSTRAINT [DF_reports_total_rental_schedule_payment] DEFAULT ((0)) NOT NULL,
    [reporting_date]       DATETIME        NOT NULL,
    [transaction_division] VARCHAR (50)    NOT NULL
);

