CREATE TABLE [dbo].[propctrl_sync_date] (
    [sync_id]   BIGINT   IDENTITY (1, 1) NOT NULL,
    [sync_date] DATETIME DEFAULT (getdate()) NOT NULL,
    PRIMARY KEY CLUSTERED ([sync_id] ASC)
);

