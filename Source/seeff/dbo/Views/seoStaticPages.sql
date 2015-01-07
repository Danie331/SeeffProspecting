CREATE VIEW dbo.seoStaticPages
AS
SELECT     dbo.seo_pages.seo_page_branchId AS seoBranchId, dbo.seo_pages.seo_page_areaId AS seoAreaId, dbo.branch.fkAreaId AS branchAreaId, 
                      dbo.branch.branchActive, dbo.branch.branchName, dbo.area.areaName, dbo.seo_pages.seo_page_region AS seoRegion, 
                      dbo.seo_pages.seo_page_url AS seoUrl
FROM         dbo.seo_pages INNER JOIN
                      dbo.branch ON dbo.seo_pages.seo_page_branchId = dbo.branch.branchId INNER JOIN
                      dbo.area ON dbo.seo_pages.seo_page_areaId = dbo.area.areaId
