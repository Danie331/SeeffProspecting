CREATE VIEW dbo.development_agents
AS
SELECT     dbo.development.developmentName, dbo.property.propertyReference, dbo.agent.agentFirstName, dbo.agent.agentSurname, 
                      dbo.development.developmentActive, dbo.development.developmentWebReference, dbo.area.areaName
FROM         dbo.area INNER JOIN
                      dbo.development ON dbo.area.areaId = dbo.development.fkAreaId LEFT OUTER JOIN
                      dbo.property LEFT OUTER JOIN
                      dbo.agent ON dbo.property.fkAgentId = dbo.agent.agentId ON dbo.development.developmentWebReference = dbo.property.propertyReference
