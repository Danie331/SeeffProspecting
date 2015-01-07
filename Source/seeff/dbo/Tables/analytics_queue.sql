CREATE TABLE [dbo].[analytics_queue] (
    [fkWebReferenceId]             INT          NOT NULL,
    [analytics_queue_category]     VARCHAR (50) NOT NULL,
    [analytics_queue_action]       VARCHAR (50) NOT NULL,
    [analytics_queue_uniqueEvents] INT          NOT NULL,
    [analytics_queue_totalEvents]  INT          NOT NULL,
    [analytics_queue_date]         VARCHAR (50) NOT NULL
);

