CREATE TABLE [dbo].[agent_category] (
    [agent_categoryId] INT IDENTITY (1, 1) NOT NULL,
    [fkAgentId]        INT CONSTRAINT [DF_agent_category_fkAgentId] DEFAULT (0) NOT NULL,
    [fkCategoryId]     INT CONSTRAINT [DF_agent_category_fkCategoryId] DEFAULT (0) NOT NULL,
    CONSTRAINT [PK_agent_category] PRIMARY KEY CLUSTERED ([agent_categoryId] ASC) WITH (FILLFACTOR = 90)
);

