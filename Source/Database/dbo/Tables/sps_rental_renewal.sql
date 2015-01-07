CREATE TABLE [dbo].[sps_rental_renewal] (
    [sps_rental_renewal_id] BIGINT          IDENTITY (1, 1) NOT NULL,
    [sps_transaction_ref]   VARCHAR (50)    NOT NULL,
    [sps_year_month]        DATETIME        NOT NULL,
    [sps_renewal_amount]    DECIMAL (18, 2) NOT NULL
);

