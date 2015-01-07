CREATE VIEW dbo.testYz
AS
SELECT     dbo.branch.branchName, dbo.area.areaName, dbo.property.propertyReference, dbo.property.propertyShortDescription, dbo.property.propertyActive, 
                      dbo.branch.branchId
FROM         dbo.property INNER JOIN
                      dbo.branch ON dbo.property.fkBranchId = dbo.branch.branchId INNER JOIN
                      dbo.area ON dbo.property.fkAreaId = dbo.area.areaId
WHERE     (dbo.branch.branchId = 110)
