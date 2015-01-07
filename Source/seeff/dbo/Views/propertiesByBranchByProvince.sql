
CREATE VIEW dbo.propertiesByBranchByProvince
AS
SELECT     TOP 100 PERCENT area_2.areaName AS Country, dbo.area.areaName AS Province, dbo.branch.branchName, area_1.areaName AS Suburb, 
                      COUNT(dbo.search.searchReference) AS activeProperties
FROM         dbo.area INNER JOIN
                      dbo.search ON dbo.area.areaId = dbo.search.searchProvince INNER JOIN
                      dbo.area area_1 ON dbo.search.searchSuburb = area_1.areaId INNER JOIN
                      dbo.branch ON dbo.search.fkBranchId = dbo.branch.branchId INNER JOIN
                      dbo.area area_2 ON dbo.search.searchCountry = area_2.areaId
GROUP BY area_2.areaName, dbo.area.areaName, dbo.branch.branchName, area_1.areaName
ORDER BY area_2.areaName, dbo.area.areaName, dbo.branch.branchName, area_1.areaName

