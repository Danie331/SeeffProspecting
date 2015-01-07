CREATE TABLE [dbo].[propctrl_update_queue] (
    [propctrl_mandate_id] INT      NOT NULL,
    [updated_date]        DATETIME NOT NULL,
    [created_date]        DATETIME CONSTRAINT [DF_propctrl_update_queue_created_date] DEFAULT (getdate()) NOT NULL
);

