CREATE TABLE [dbo].[reports_daily_track] (
    [report_date]  VARCHAR (50) NOT NULL,
    [report_run]   BIT          NOT NULL,
    [created_date] DATETIME     CONSTRAINT [DF_reports_daily_track_created_date] DEFAULT (getdate()) NOT NULL
);

