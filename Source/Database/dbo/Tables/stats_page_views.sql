CREATE TABLE [dbo].[stats_page_views] (
    [page_view_id]                  BIGINT   IDENTITY (1, 1) NOT NULL,
    [no_visits]                     INT      NOT NULL,
    [no_page_views]                 INT      NOT NULL,
    [previous_days_email_enquiries] INT      NOT NULL,
    [previous_days_sms_enquiries]   INT      NOT NULL,
    [stat_date]                     DATETIME NOT NULL
);

