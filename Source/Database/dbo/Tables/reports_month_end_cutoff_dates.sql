CREATE TABLE [dbo].[reports_month_end_cutoff_dates] (
    [Cutoff_Date_ID]     INT          IDENTITY (1, 1) NOT NULL,
    [First_of_Month]     DATETIME     NOT NULL,
    [First_Day_of_Month] NVARCHAR (5) NOT NULL,
    [Cut_off_Date]       DATETIME     NOT NULL,
    [Cut_off_Day]        NVARCHAR (5) NOT NULL,
    [reporting_year]     INT          NULL,
    [reporting_month]    INT          NULL
);

