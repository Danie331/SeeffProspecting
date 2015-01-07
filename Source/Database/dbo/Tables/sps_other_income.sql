CREATE TABLE [dbo].[sps_other_income] (
    [other_income_id] INT             IDENTITY (1, 1) NOT NULL,
    [license_id]      INT             NOT NULL,
    [reporting_year]  INT             NOT NULL,
    [reporting_month] INT             NOT NULL,
    [reported_amount] DECIMAL (18, 2) NULL,
    [reported_date]   DATETIME        NULL,
    [reported_by]     INT             NULL,
    [description]     VARCHAR (500)   CONSTRAINT [DF_sps_other_income_description] DEFAULT ('Other Income') NOT NULL
);

