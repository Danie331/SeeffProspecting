CREATE TABLE [dbo].[reports_auto_letters] (
    [report_id]       INT           IDENTITY (1, 1) NOT NULL,
    [registration_id] INT           NOT NULL,
    [seller]          VARCHAR (150) NOT NULL,
    [address]         VARCHAR (250) NOT NULL,
    [pdf_path]        VARCHAR (250) NOT NULL,
    [seller_email]    VARCHAR (250) NOT NULL,
    [bcc_email]       VARCHAR (250) NULL,
    [report_name]     VARCHAR (250) NOT NULL,
    [created_date]    DATETIME      NOT NULL
);

