CREATE TABLE [dbo].[kpi_branch_month_end_total] (
    [kpi_id]                                   INT         IDENTITY (1, 1) NOT NULL,
    [fk_branch_id]                             INT         NOT NULL,
    [active_agents_sales]                      INT         NULL,
    [active_agents_rentals]                    INT         NULL,
    [active_new_listings_monthly_sales]        INT         NULL,
    [active_new_listings_monthly_rentals]      INT         NULL,
    [active_total_listings_sales]              INT         NULL,
    [active_total_listings_rentals]            INT         NULL,
    [active_new_sole_mandates_monthly_sales]   INT         NULL,
    [active_new_sole_mandates_monthly_rentals] INT         NULL,
    [active_total_sole_mandates_sales]         INT         NULL,
    [active_total_sole_mandates_rentals]       INT         NULL,
    [show_days_new_monthly_sales]              INT         NULL,
    [show_days_new_monthly_rentals]            INT         NULL,
    [open_hours_new_monthly_sales]             INT         NULL,
    [open_hours_new_monthly_rentals]           INT         NULL,
    [kpi_month]                                VARCHAR (2) NOT NULL,
    [kpi_year]                                 VARCHAR (4) NOT NULL,
    [recorded_date]                            DATETIME    NOT NULL,
    PRIMARY KEY CLUSTERED ([kpi_id] ASC)
);

