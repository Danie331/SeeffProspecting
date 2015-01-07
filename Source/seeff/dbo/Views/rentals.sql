
CREATE VIEW dbo.rentals
AS
SELECT     TOP 100 PERCENT dbo.area.areaName AS Province, dbo.branch.branchName, area_1.areaName AS Suburb, dbo.search.searchReference, 
                      dbo.search.searchPrice, dbo.agent.agentFirstName, dbo.agent.agentSurname, dbo.agent.agentTelephone, dbo.agent.agentCell, 
                      dbo.propertyType.propertyTypeName
FROM         dbo.area INNER JOIN
                      dbo.search ON dbo.area.areaId = dbo.search.searchProvince INNER JOIN
                      dbo.area area_1 ON dbo.search.searchSuburb = area_1.areaId INNER JOIN
                      dbo.branch ON dbo.search.fkBranchId = dbo.branch.branchId INNER JOIN
                      dbo.property ON dbo.search.fkPropertyId = dbo.property.propertyId INNER JOIN
                      dbo.agent ON dbo.property.fkAgentId = dbo.agent.agentId INNER JOIN
                      dbo.propertyType ON dbo.search.fkPropertyTypeId = dbo.propertyType.propertyTypeId
WHERE     (dbo.search.fkCategoryId = 6)
ORDER BY dbo.area.areaName, dbo.branch.branchName, area_1.areaName

