-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spMIS_RA_National_Rand_YTD] 
	-- Add the parameters for the stored procedure here
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here	
     
	SELECT
		RANK() OVER ( ORDER BY SUM([Recognition Amount]) DESC) AS Ranking
		,License
		,Agent
		,SUM([Recognition Amount]) AS [Recognition Amount]		
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
		,SUM([Recognition Amount])		
FROM 
		dbo.reports_Sales_Recognition_Detail_Global			
END
