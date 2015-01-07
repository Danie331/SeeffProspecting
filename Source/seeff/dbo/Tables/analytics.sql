CREATE TABLE [dbo].[analytics] (
    [analytics_id]           INT          IDENTITY (1, 1) NOT NULL,
    [fkWebReferenceId]       INT          NULL,
    [fkBranchId]             INT          NULL,
    [fkAreaId]               INT          NULL,
    [fkAgentId]              INT          NULL,
    [propertyPrice]          VARCHAR (50) NULL,
    [analytics_category]     VARCHAR (50) NULL,
    [analytics_action]       VARCHAR (50) NULL,
    [analytics_uniqueEvents] INT          NULL,
    [analytics_totalEvents]  INT          NULL,
    [analytics_date]         VARCHAR (50) NULL,
    CONSTRAINT [PK_analytics] PRIMARY KEY CLUSTERED ([analytics_id] ASC)
);

