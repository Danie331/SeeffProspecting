CREATE TABLE [dbo].[property_agent] (
    [property_agent_id]     BIGINT   IDENTITY (1, 1) NOT NULL,
    [fk_property_reference] INT      NOT NULL,
    [fk_agent_id]           INT      NOT NULL,
    [inserted_date]         DATETIME DEFAULT (getdate()) NOT NULL,
    PRIMARY KEY CLUSTERED ([property_agent_id] ASC)
);

