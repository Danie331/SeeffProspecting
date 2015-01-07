CREATE VIEW dbo.propertiesPerBranch
AS
SELECT     TOP 100 PERCENT branchName, branchId,
                          (SELECT     COUNT(fkBranchId)
                            FROM          search
                            WHERE      fkBranchId = branchId
                            GROUP BY fkBranchId) AS activeProperties
FROM         dbo.branch
ORDER BY branchName
