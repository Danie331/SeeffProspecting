CREATE TABLE [dbo].[reports_manager_summary] (
    [manager_reports_id] INT            IDENTITY (1, 1) NOT NULL,
    [license_id]         INT            NOT NULL,
    [manager_id]         INT            NOT NULL,
    [html_report]        NVARCHAR (MAX) NOT NULL
);

