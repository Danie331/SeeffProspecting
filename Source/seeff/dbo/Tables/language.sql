CREATE TABLE [dbo].[language] (
    [languageId]   INT           IDENTITY (1, 1) NOT NULL,
    [languageName] NVARCHAR (50) NULL,
    CONSTRAINT [PK_language] PRIMARY KEY CLUSTERED ([languageId] ASC) WITH (FILLFACTOR = 90)
);

