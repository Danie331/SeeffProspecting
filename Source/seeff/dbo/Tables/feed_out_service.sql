CREATE TABLE [dbo].[feed_out_service] (
    [feed_id]       INT           IDENTITY (1, 1) NOT NULL,
    [province]      VARCHAR (20)  NOT NULL,
    [seeff_web_ref] INT           NOT NULL,
    [branch_id]     INT           NOT NULL,
    [branch_name]   VARCHAR (MAX) NOT NULL,
    [seeff_feed]    VARCHAR (MAX) NULL,
    [iol_feed]      VARCHAR (MAX) NULL,
    PRIMARY KEY CLUSTERED ([feed_id] ASC)
);

