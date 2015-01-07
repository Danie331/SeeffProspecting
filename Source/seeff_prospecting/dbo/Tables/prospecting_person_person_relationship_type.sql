CREATE TABLE [dbo].[prospecting_person_person_relationship_type] (
    [person_person_relationship_type_id] INT           IDENTITY (1, 1) NOT NULL,
    [relationship_desc]                  VARCHAR (255) NOT NULL,
    PRIMARY KEY CLUSTERED ([person_person_relationship_type_id] ASC)
);

