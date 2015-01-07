CREATE TABLE [dbo].[propctrl_log] (
    [log_id]                 BIGINT        IDENTITY (1, 1) NOT NULL,
    [fk_propctrl_mandate_id] INT           NULL,
    [fk_property_reference]  INT           NULL,
    [content]                VARCHAR (MAX) NULL,
    [created_date]           DATETIME      DEFAULT (getdate()) NOT NULL,
    PRIMARY KEY CLUSTERED ([log_id] ASC)
);

