CREATE TABLE [dbo].[prospecting_person_title] (
    [prospecting_person_title_id] INT         IDENTITY (1, 1) NOT NULL,
    [person_title]                VARCHAR (5) NOT NULL,
    PRIMARY KEY CLUSTERED ([prospecting_person_title_id] ASC)
);

