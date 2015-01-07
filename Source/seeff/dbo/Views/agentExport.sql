CREATE VIEW dbo.agentExport
AS
SELECT     TOP 100 PERCENT dbo.branch.branchName, dbo.agentType.agentTypeName, dbo.agent.agentFirstName, dbo.agent.agentSurname, 
                      dbo.agent.agentEmail, { fn CONCAT('`', dbo.agent.agentCell) } AS agentCell
FROM         dbo.agent INNER JOIN
                      dbo.agentType ON dbo.agent.fkAgentTypeId = dbo.agentType.agentTypeId INNER JOIN
                      dbo.branch ON dbo.agent.fkBranchId = dbo.branch.branchId
ORDER BY dbo.branch.branchName, dbo.agentType.agentTypeName, dbo.agent.agentFirstName
