CREATE TABLE [dbo].[prospecting_person_company_relationship] (
    [person_company_relationship_id] INT              IDENTITY (1, 1) NOT NULL,
    [contact_person_id]              INT              NOT NULL,
    [contact_company_id]             INT              NULL,
    [relationship_to_company]        INT              NULL,
    [created_date]                   DATETIME         NULL,
    [updated_date]                   DATETIME         NULL,
    [created_by]                     UNIQUEIDENTIFIER NULL,
    PRIMARY KEY CLUSTERED ([person_company_relationship_id] ASC),
    FOREIGN KEY ([contact_company_id]) REFERENCES [dbo].[prospecting_contact_company] ([contact_company_id]),
    FOREIGN KEY ([contact_person_id]) REFERENCES [dbo].[prospecting_contact_person] ([contact_person_id]),
    FOREIGN KEY ([relationship_to_company]) REFERENCES [dbo].[prospecting_person_company_relationship_type] ([person_company_relationship_type_id])
);

