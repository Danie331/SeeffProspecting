CREATE TABLE [dbo].[prospecting_contact_company] (
    [contact_company_id] INT              IDENTITY (1, 1) NOT NULL,
    [company_name]       VARCHAR (255)    NOT NULL,
    [CK_number]          VARCHAR (255)    NULL,
    [created_date]       DATETIME         NULL,
    [updated_date]       DATETIME         NULL,
    [created_by]         UNIQUEIDENTIFIER NULL,
    [company_type]       VARCHAR (10)     NULL,
    PRIMARY KEY CLUSTERED ([contact_company_id] ASC)
);

