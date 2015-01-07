CREATE VIEW dbo.[province list]
AS
SELECT     areaName AS Province, areaId
FROM         dbo.area
WHERE     (fkAreaTypeId = 2)
