CREATE TABLE [dbo].[prospecting_person_person_relationship] (
    [person_person_relationship_id] INT              IDENTITY (1, 1) NOT NULL,
    [contact_person_id]             INT              NOT NULL,
    [related_contacted_person_id]   INT              NOT NULL,
    [relationship_to_person]        INT              NOT NULL,
    [created_date]                  DATETIME         NULL,
    [updated_date]                  DATETIME         NULL,
    [created_by]                    UNIQUEIDENTIFIER NULL,
    PRIMARY KEY CLUSTERED ([person_person_relationship_id] ASC),
    FOREIGN KEY ([contact_person_id]) REFERENCES [dbo].[prospecting_contact_person] ([contact_person_id]),
    FOREIGN KEY ([related_contacted_person_id]) REFERENCES [dbo].[prospecting_contact_person] ([contact_person_id]),
    FOREIGN KEY ([relationship_to_person]) REFERENCES [dbo].[prospecting_person_person_relationship_type] ([person_person_relationship_type_id])
);

