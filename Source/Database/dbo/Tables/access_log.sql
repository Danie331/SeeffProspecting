CREATE TABLE [dbo].[access_log] (
    [registration_id] INT           NOT NULL,
    [access_date]     DATETIME      CONSTRAINT [DF_access_log_access_date] DEFAULT (getdate()) NOT NULL,
    [page]            VARCHAR (100) NOT NULL
);

