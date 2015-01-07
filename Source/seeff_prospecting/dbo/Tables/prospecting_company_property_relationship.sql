CREATE TABLE [dbo].[prospecting_company_property_relationship] (
    [company_property_relationship_id] INT              IDENTITY (1, 1) NOT NULL,
    [contact_company_id]               INT              NOT NULL,
    [prospecting_property_id]          INT              NOT NULL,
    [relationship_to_property]         INT              NOT NULL,
    [created_date]                     DATETIME         NULL,
    [updated_date]                     DATETIME         NULL,
    [created_by]                       UNIQUEIDENTIFIER NULL,
    PRIMARY KEY CLUSTERED ([company_property_relationship_id] ASC),
    FOREIGN KEY ([contact_company_id]) REFERENCES [dbo].[prospecting_contact_company] ([contact_company_id]),
    FOREIGN KEY ([relationship_to_property]) REFERENCES [dbo].[prospecting_company_property_relationship_type] ([company_property_relationship_type_id]),
    CONSTRAINT [FK__prospecti__prosp__398D8EEE] FOREIGN KEY ([prospecting_property_id]) REFERENCES [dbo].[prospecting_property] ([prospecting_property_id])
);

