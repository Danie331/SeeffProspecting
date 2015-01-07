CREATE TABLE [dbo].[analytics_daily] (
    [pk_analytics_daily_id] INT           IDENTITY (1, 1) NOT NULL,
    [daily_fkBranchId]      INT           NOT NULL,
    [daily_branchName]      VARCHAR (255) NOT NULL,
    [daily_action]          VARCHAR (255) NOT NULL,
    [daily_date]            VARCHAR (255) NOT NULL,
    [daily_uniqueEvents]    INT           NOT NULL,
    [daily_totalEvents]     INT           NOT NULL
);

