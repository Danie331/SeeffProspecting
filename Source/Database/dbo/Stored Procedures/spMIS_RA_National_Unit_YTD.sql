-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spMIS_RA_National_Unit_YTD] 
	-- Add the parameters for the stored procedure here
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here	
     
	SELECT
		RANK() OVER ( ORDER BY SUM([Recognition Unit]) DESC) AS Ranking
		,License
		,Agent
		,SUM([Recognition Unit]) AS [Recognition Unit]		
FROM 
		dbo.reports_Sales_Recognition_Detail_Global
WHERE
		[Recognition Amount] > 0
AND
		[Recognition Unit] > 0		
GROUP BY
		License
		,Agent
				
UNION ALL

SELECT
		1 AS Ranking
		,'' AS License
		,NULL AS Agent
		,SUM([Recognition Unit])		
FROM 
		dbo.reports_Sales_Recognition_Detail_Global			
END
