CREATE VIEW dbo.[NumberOfPropertiesByBranch]
AS
SELECT     dbo.branch.branchName, COUNT(dbo.property.propertyReference) AS NumberOfProperties
FROM         dbo.property INNER JOIN
                      dbo.branch ON dbo.property.fkBranchId = dbo.branch.branchId
GROUP BY dbo.branch.branchName
