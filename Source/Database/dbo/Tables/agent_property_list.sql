CREATE TABLE [dbo].[agent_property_list] (
    [fkAgentId]    INT      NOT NULL,
    [propertyId]   BIGINT   NOT NULL,
    [sequence]     INT      IDENTITY (1, 1) NOT NULL,
    [created_date] DATETIME CONSTRAINT [DF_agent_property_list_created_date] DEFAULT (getdate()) NOT NULL
);

