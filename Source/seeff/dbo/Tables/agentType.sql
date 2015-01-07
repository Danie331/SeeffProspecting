CREATE TABLE [dbo].[agentType] (
    [agentTypeId]   INT           IDENTITY (1, 1) NOT NULL,
    [agentTypeName] NVARCHAR (50) NULL,
    CONSTRAINT [PK_agentType] PRIMARY KEY CLUSTERED ([agentTypeId] ASC) WITH (FILLFACTOR = 90)
);

