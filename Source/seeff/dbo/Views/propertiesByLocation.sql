CREATE VIEW dbo.propertiesByLocation
AS
SELECT     TOP 100 PERCENT dbo.area.areaName, dbo.area.areaId,
                          (SELECT     COUNT(fkAreaId)
                            FROM          search
                            WHERE      fkAreaId = areaId
                            GROUP BY fkAreaId) AS activeProperties, dbo.branch.branchName, dbo.branch_area.fkAreaId AS fkAreaIdBranchArea, 
                      dbo.branch_area.branch_areaId, dbo.branch_area.fkBranchId AS fkBranchIdBranchArea, dbo.property.propertyId, dbo.property.propertyReference, 
                      dbo.property.fkBranchId AS fkBranchIdProperty, dbo.property.fkAreaId AS fkAreaIdProperty, dbo.branch.branchId, dbo.area.fkAreaTypeId, 
                      dbo.property.propertyActive
FROM         dbo.area INNER JOIN
                      dbo.property ON dbo.area.areaId = dbo.property.fkAreaId AND dbo.property.propertyActive = 1 INNER JOIN
                      dbo.branch ON dbo.property.fkBranchId = dbo.branch.branchId INNER JOIN
                      dbo.branch_area ON dbo.area.areaId = dbo.branch_area.fkAreaId AND dbo.branch.branchId = dbo.branch_area.fkBranchId
ORDER BY dbo.area.areaName
