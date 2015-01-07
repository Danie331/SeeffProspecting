CREATE VIEW dbo.seomapping
AS
SELECT     dbo.branch.branchName, dbo.branch.branchURL, dbo.seoUrl.seoUrl, dbo.seoUrl.seoGoto
FROM         dbo.branch LEFT OUTER JOIN
                      dbo.seoUrl ON dbo.branch.branchURL = dbo.seoUrl.seoUrl
