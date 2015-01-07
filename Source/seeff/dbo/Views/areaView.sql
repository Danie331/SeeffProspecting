
CREATE VIEW dbo.areaView
AS
SELECT     dbo.AreaMap.fkAreaId, dbo.AreaMap.areaName, dbo.area.areaName + ' (' + AreaMap_1.areaName + ')' AS parentAreaName
FROM         dbo.AreaMap INNER JOIN
                      dbo.area ON dbo.AreaMap.fkAreaParentId = dbo.area.areaId INNER JOIN
                      dbo.AreaMap AreaMap_1 ON dbo.area.areaParentId = AreaMap_1.fkAreaId

