CREATE TABLE [dbo].[agent_language] (
    [agent_languageId] INT IDENTITY (1, 1) NOT NULL,
    [fkAgentId]        INT NULL,
    [fkLanguageId]     INT NULL,
    CONSTRAINT [PK_agent_language] PRIMARY KEY CLUSTERED ([agent_languageId] ASC) WITH (FILLFACTOR = 90)
);

