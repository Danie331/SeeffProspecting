CREATE VIEW dbo.getEmploymentOpportunities
AS
SELECT     dbo.employmentOpportunity.employmentOpportunityTitle, dbo.employmentOpportunity.employmentOpportunityDescription, dbo.branch.branchName, 
                      dbo.area.areaId, dbo.agent.agentFirstName, dbo.agent.agentSurname, dbo.agent.agentEmail, dbo.agent.agentTelephone,
                          (SELECT     areaName
                            FROM          area
                            WHERE      fkAreaTypeId = 2 AND PATINDEX(('%|' + cast(areaId AS varchar(10)) + '|%'),
                                                       (SELECT     sPath
                                                         FROM          AreaMap
                                                         WHERE      PATINDEX('%|' + cast(dbo.area.areaId AS varchar(10)) + '|', sPath) > 0)) > 0) AS Province
FROM         dbo.employmentOpportunity INNER JOIN
                      dbo.branch ON dbo.employmentOpportunity.fkBranchId = dbo.branch.branchId INNER JOIN
                      dbo.area ON dbo.branch.fkAreaId = dbo.area.areaId INNER JOIN
                      dbo.agent ON dbo.employmentOpportunity.fkAgentId = dbo.agent.agentId
