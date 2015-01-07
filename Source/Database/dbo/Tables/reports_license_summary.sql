CREATE TABLE [dbo].[reports_license_summary] (
    [license_summary_id] INT             IDENTITY (1, 1) NOT NULL,
    [measurement_desc]   NVARCHAR (150)  NOT NULL,
    [year]               INT             NOT NULL,
    [license_id]         INT             NOT NULL,
    [branch_id]          INT             NOT NULL,
    [jan]                DECIMAL (18, 2) CONSTRAINT [DF_reports_license_summary_jan] DEFAULT ((0)) NOT NULL,
    [feb]                DECIMAL (18, 2) CONSTRAINT [DF_reports_license_summary_feb] DEFAULT ((0)) NOT NULL,
    [mar]                DECIMAL (18, 2) CONSTRAINT [DF_reports_license_summary_mar] DEFAULT ((0)) NOT NULL,
    [apr]                DECIMAL (18, 2) CONSTRAINT [DF_reports_license_summary_apr] DEFAULT ((0)) NOT NULL,
    [may]                DECIMAL (18, 2) CONSTRAINT [DF_reports_license_summary_may] DEFAULT ((0)) NOT NULL,
    [jun]                DECIMAL (18, 2) CONSTRAINT [DF_reports_license_summary_jun] DEFAULT ((0)) NOT NULL,
    [jul]                DECIMAL (18, 2) CONSTRAINT [DF_reports_license_summary_jul] DEFAULT ((0)) NOT NULL,
    [aug]                DECIMAL (18, 2) CONSTRAINT [DF_reports_license_summary_aug] DEFAULT ((0)) NOT NULL,
    [sep]                DECIMAL (18, 2) CONSTRAINT [DF_reports_license_summary_sep] DEFAULT ((0)) NOT NULL,
    [oct]                DECIMAL (18, 2) CONSTRAINT [DF_reports_license_summary_oct] DEFAULT ((0)) NOT NULL,
    [nov]                DECIMAL (18, 2) CONSTRAINT [DF_reports_license_summary_nov] DEFAULT ((0)) NOT NULL,
    [dec]                DECIMAL (18, 2) CONSTRAINT [DF_reports_license_summary_dec] DEFAULT ((0)) NOT NULL,
    [total]              DECIMAL (18, 2) CONSTRAINT [DF_reports_license_summary_total] DEFAULT ((0)) NOT NULL,
    [registration_id]    INT             NOT NULL
);

