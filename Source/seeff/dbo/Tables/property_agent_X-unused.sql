CREATE TABLE [dbo].[property_agent_X-unused] (
    [property_agentId] NUMERIC (18) IDENTITY (1, 1) NOT NULL,
    [fkPropertyId]     NUMERIC (18) CONSTRAINT [DF_property_agent_fkPropertyId] DEFAULT (0) NOT NULL,
    [fkAgentId]        INT          CONSTRAINT [DF_property_agent_fkAgentId] DEFAULT (0) NOT NULL,
    CONSTRAINT [PK_property_agent] PRIMARY KEY CLUSTERED ([property_agentId] ASC) WITH (FILLFACTOR = 90)
);

