CREATE TABLE [dbo].[sps_person] (
    [sps_person_id]        INT           IDENTITY (1, 1) NOT NULL,
    [sps_title]            VARCHAR (20)  NOT NULL,
    [sps_name]             VARCHAR (150) NOT NULL,
    [sps_surname]          VARCHAR (150) NOT NULL,
    [sps_company]          VARCHAR (250) NULL,
    [sps_country_code]     VARCHAR (10)  NOT NULL,
    [sps_cellno]           VARCHAR (10)  NOT NULL,
    [sps_email_address]    VARCHAR (250) NULL,
    [sps_id]               VARCHAR (20)  NULL,
    [branchId]             INT           NOT NULL,
    [created_date]         DATETIME      CONSTRAINT [DF_sps_person_time_stamp] DEFAULT (getdate()) NOT NULL,
    [updated_date]         DATETIME      CONSTRAINT [DF_sps_person_updated_date] DEFAULT (getdate()) NOT NULL,
    [ethnic_group]         VARCHAR (50)  NULL,
    [smart_pass_person_id] INT           NULL
);

