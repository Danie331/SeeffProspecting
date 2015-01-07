CREATE VIEW dbo.branch_details_export
AS
SELECT     TOP 100 PERCENT dbo.branch.branchId, dbo.branch.branchName, dbo.branch.branchTelephone, dbo.branch.branchFax, dbo.branch.branchCell, 
                      dbo.branch.branchPostalAddress, dbo.branch.branchPhysicalAddress, dbo.branch.branchEmail, dbo.area.areaName, dbo.agent.agentFirstName, 
                      dbo.agent.agentSurname
FROM         dbo.branch INNER JOIN
                      dbo.area ON dbo.branch.fkAreaId = dbo.area.areaId INNER JOIN
                      dbo.agent ON dbo.branch.branchContact = dbo.agent.agentId
ORDER BY dbo.branch.branchName
