CREATE VIEW dbo.propertyExport
AS
SELECT     (SELECT     areaName
                       FROM          area
                       WHERE      fkAreaTypeId = 2 AND PATINDEX(('%|' + cast(areaId AS varchar(10)) + '|%'),
                                                  (SELECT     sPath
                                                    FROM          AreaMap
                                                    WHERE      PATINDEX('%|' + cast(dbo.area.areaId AS varchar(10)) + '|', sPath) > 0)) > 0) AS Province, dbo.area.areaName, 
                      dbo.agent.agentId, dbo.agent.agentFirstName, dbo.agent.agentSurname, dbo.agent.agentCell, dbo.branch.branchName, 
                      dbo.propertyType.propertyTypeName, dbo.property.propertyReference, dbo.property.propertyPrice, dbo.property.propertyRatesAndTaxes, 
                      dbo.property.propertyMandate, dbo.property.propertySellerName, dbo.property.propertyTelephoneAreaCode, dbo.property.propertyTelephone, 
                      dbo.property.propertyActive, dbo.property.propertyOnShow, dbo.branch.branchId, dbo.property.propertyAddress, dbo.property.propertyId , 
                     (SELECT COUNT(property_imagesName) AS ImageTotal FROM dbo.property_images WHERE propertyReference = dbo.property.propertyReference) AS numImages
FROM         dbo.property INNER JOIN
                      dbo.agent ON dbo.property.fkAgentId = dbo.agent.agentId INNER JOIN
                      dbo.branch ON dbo.property.fkBranchId = dbo.branch.branchId INNER JOIN
                      dbo.propertyType ON dbo.property.fkPropertyTypeId = dbo.propertyType.propertyTypeId INNER JOIN
                      dbo.area ON dbo.property.fkAreaId = dbo.area.areaId
WHERE     ((SELECT     areaName
                         FROM         area
                         WHERE     fkAreaTypeId = 2 AND PATINDEX(('%|' + cast(areaId AS varchar(10)) + '|%'),
                                                   (SELECT     sPath
                                                     FROM          AreaMap
                                                     WHERE      PATINDEX('%|' + cast(dbo.area.areaId AS varchar(10)) + '|', sPath) > 0)) > 0) IS NOT NULL)

