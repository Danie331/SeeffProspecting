CREATE TABLE [dbo].[market_share_area_totals] (
    [ms_id]             INT             IDENTITY (1, 1) NOT NULL,
    [ms_total_count]    INT             NOT NULL,
    [ms_total_value]    DECIMAL (18)    NOT NULL,
    [ms_seeff_count]    INT             NOT NULL,
    [ms_seeff_value]    DECIMAL (18)    NOT NULL,
    [unit_market_share] DECIMAL (18, 4) NOT NULL,
    [rand_market_share] DECIMAL (18, 4) NOT NULL,
    [ms_property_type]  VARCHAR (2)     NOT NULL,
    [ms_year]           VARCHAR (4)     NOT NULL,
    [ms_area_id]        INT             NOT NULL,
    [ms_license_id]     INT             NOT NULL
);

