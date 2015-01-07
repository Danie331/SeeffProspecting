CREATE TABLE [dbo].[agentRole] (
    [agentRoleId]   INT           IDENTITY (1, 1) NOT NULL,
    [agentRoleName] NVARCHAR (50) NULL,
    CONSTRAINT [PK_agentRole] PRIMARY KEY CLUSTERED ([agentRoleId] ASC) WITH (FILLFACTOR = 90)
);

