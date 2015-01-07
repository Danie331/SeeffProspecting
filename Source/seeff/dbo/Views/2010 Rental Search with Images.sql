CREATE VIEW dbo.[2010 Rental Search with Images]
AS
SELECT     areaName AS Province, areaId
FROM         dbo.area
WHERE     (fkAreaTypeId = 2)
