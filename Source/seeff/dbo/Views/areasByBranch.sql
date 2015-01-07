CREATE VIEW dbo.areasByBranch
AS
SELECT     dbo.area.areaId, dbo.area.areaName, dbo.property.fkBranchId, dbo.property.fkCategoryId
FROM         dbo.area INNER JOIN
                      dbo.property ON dbo.area.areaId = dbo.property.fkAreaId
GROUP BY dbo.area.areaName, dbo.property.fkBranchId, dbo.property.fkCategoryId, dbo.area.areaId
