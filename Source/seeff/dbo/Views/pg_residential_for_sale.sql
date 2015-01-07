CREATE VIEW dbo.pg_residential_for_sale
AS
SELECT     dbo.area.areaId, dbo.area.areaName, dbo.search.searchReference, dbo.search.searchPropertySold, dbo.category.categoryName, 
                      dbo.category.categoryId, dbo.[action].actionId, dbo.[action].actionName
FROM         dbo.search INNER JOIN
                      dbo.area ON dbo.search.searchCountry = dbo.area.areaId INNER JOIN
                      dbo.category ON dbo.search.fkCategoryId = dbo.category.categoryId INNER JOIN
                      dbo.[action] ON dbo.search.fkActionId = dbo.[action].actionId
WHERE     (dbo.search.searchPropertySold <> 1) AND (dbo.category.categoryId = 1) AND (dbo.[action].actionId = 2) AND (dbo.area.areaId = 1)
