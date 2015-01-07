CREATE VIEW dbo.branchStaticPageSuburb
AS
SELECT     TOP 100 PERCENT dbo.branch.branchName, dbo.area.areaName AS suburb, dbo.seo_pages.seo_page_url AS staticPage, dbo.branch.branchActive, 
                      dbo.areaType.areaTypeName, dbo.branch.branchId
FROM         dbo.branch INNER JOIN
                      dbo.branch_area ON dbo.branch.branchId = dbo.branch_area.fkBranchId LEFT OUTER JOIN
                      dbo.area ON dbo.branch_area.fkAreaId = dbo.area.areaId AND dbo.branch.fkAreaId = dbo.area.areaId LEFT OUTER JOIN
                      dbo.seo_pages ON dbo.branch.branchId = dbo.seo_pages.seo_page_branchId AND 
                      dbo.area.areaId = dbo.seo_pages.seo_page_areaId LEFT OUTER JOIN
                      dbo.areaType ON dbo.area.fkAreaTypeId = dbo.areaType.areaTypeId AND dbo.areaType.areaTypeName IS NOT NULL
WHERE     (dbo.areaType.areaTypeName IS NOT NULL)
GROUP BY dbo.branch.branchId, dbo.branch.branchName, dbo.area.areaId, dbo.area.areaName, dbo.seo_pages.seo_page_url, dbo.branch.branchActive, 
                      dbo.areaType.areaTypeName
ORDER BY dbo.branch.branchName, dbo.area.areaName, dbo.seo_pages.seo_page_url
