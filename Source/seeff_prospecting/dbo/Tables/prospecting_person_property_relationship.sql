CREATE TABLE [dbo].[prospecting_person_property_relationship] (
    [person_property_relationship_id] INT              IDENTITY (1, 1) NOT NULL,
    [contact_person_id]               INT              NOT NULL,
    [prospecting_property_id]         INT              NOT NULL,
    [relationship_to_property]        INT              NOT NULL,
    [created_date]                    DATETIME         NULL,
    [updated_date]                    DATETIME         NULL,
    [created_by]                      UNIQUEIDENTIFIER NULL,
    [from_date]                       DATETIME         NULL,
    [to_date]                         DATETIME         NULL,
    PRIMARY KEY CLUSTERED ([person_property_relationship_id] ASC),
    FOREIGN KEY ([contact_person_id]) REFERENCES [dbo].[prospecting_contact_person] ([contact_person_id]),
    FOREIGN KEY ([relationship_to_property]) REFERENCES [dbo].[prospecting_person_property_relationship_type] ([person_property_relationship_type_id]),
    CONSTRAINT [FK__prospecti__prosp__46E78A0C] FOREIGN KEY ([prospecting_property_id]) REFERENCES [dbo].[prospecting_property] ([prospecting_property_id])
);

