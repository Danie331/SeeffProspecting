CREATE TABLE [dbo].[smart_pass_person] (
    [smart_pass_person_id]     INT           IDENTITY (1, 1) NOT NULL,
    [smart_pass_title]         VARCHAR (20)  NOT NULL,
    [smart_pass_name]          VARCHAR (150) NOT NULL,
    [smart_pass_surname]       VARCHAR (150) NOT NULL,
    [smart_pass_company]       VARCHAR (250) NULL,
    [smart_pass_contact_type]  VARCHAR (50)  NOT NULL,
    [smart_pass_country_code]  VARCHAR (10)  NOT NULL,
    [smart_pass_contact_no]    VARCHAR (10)  NOT NULL,
    [smart_pass_email_address] VARCHAR (250) NULL,
    [smart_pass_id_no]         VARCHAR (20)  NULL,
    [registration_id]          INT           NOT NULL,
    [created_date]             DATETIME      CONSTRAINT [DF_smart_pass_person_time_stamp] DEFAULT (getdate()) NOT NULL,
    [updated_date]             DATETIME      CONSTRAINT [DF_smart_pass_person_updated_date] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [UNQ_smart_pass_person_smart_pass_contact_no] UNIQUE NONCLUSTERED ([smart_pass_contact_no] ASC)
);

