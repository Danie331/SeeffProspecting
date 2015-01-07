
CREATE VIEW dbo.agentView
AS
SELECT     dbo.agent.agentId, dbo.agent.agentFirstName + ' ' + dbo.agent.agentSurname AS agentName, dbo.agent.fkAgentTypeId, dbo.agent.fkAgentRoleId, 
                      dbo.branch.branchId, dbo.branch.branchName, dbo.agent.agentActive
FROM         dbo.agent INNER JOIN
                      dbo.branch ON dbo.agent.fkBranchId = dbo.branch.branchId

