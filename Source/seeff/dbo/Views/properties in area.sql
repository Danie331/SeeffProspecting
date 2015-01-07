CREATE VIEW dbo.[properties in area]
AS
SELECT     dbo.search.searchReference, dbo.property.propertyAddress, dbo.agent.agentFirstName, dbo.agent.agentSurname, agent_1.agentFirstName AS Expr1, 
                      agent_1.agentSurname AS Expr2
FROM         dbo.search INNER JOIN
                      dbo.property ON dbo.search.fkPropertyId = dbo.property.propertyId INNER JOIN
                      dbo.agent ON dbo.property.fkAgentId = dbo.agent.agentId INNER JOIN
                      dbo.agent agent_1 ON dbo.property.propertLastEditedBy = agent_1.agentId
WHERE     (dbo.search.fkAreaId = 236)
