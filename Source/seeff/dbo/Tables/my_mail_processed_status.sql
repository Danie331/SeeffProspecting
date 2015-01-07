CREATE TABLE [dbo].[my_mail_processed_status] (
    [status_id]   INT          IDENTITY (1, 1) NOT NULL,
    [status_name] VARCHAR (20) NOT NULL,
    PRIMARY KEY CLUSTERED ([status_id] ASC)
);

