CREATE TABLE [dbo].[sps_reporting_person] (
    [figures_id]      INT          IDENTITY (1, 1) NOT NULL,
    [registration_id] INT          NOT NULL,
    [created_date]    DATETIME     CONSTRAINT [DF_sps_reporting_person_created_date] DEFAULT (getdate()) NOT NULL,
    [region]          VARCHAR (50) CONSTRAINT [DF_sps_reporting_person_region] DEFAULT ('local') NOT NULL
);

