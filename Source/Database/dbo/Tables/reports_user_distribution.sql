CREATE TABLE [dbo].[reports_user_distribution] (
    [registration_id] INT NOT NULL,
    [sales_reports]   BIT CONSTRAINT [DF_reports_user_distribution_sales_reports] DEFAULT ((0)) NOT NULL,
    [rental_reports]  BIT CONSTRAINT [DF_reports_user_distribution_rental_reports] DEFAULT ((0)) NOT NULL
);

