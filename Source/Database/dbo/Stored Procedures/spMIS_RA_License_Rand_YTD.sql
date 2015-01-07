-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spMIS_RA_License_Rand_YTD] 
	-- Add the parameters for the stored procedure here
		@LicID INT = 89
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here	
     
	SELECT
		RANK() OVER (PARTITION BY [License] ORDER BY SUM([Recognition Amount]) DESC) AS Ranking
		--,License
		,Agent
		,SUM([Recognition Amount]) AS [Recognition Amount]		
FROM 
		dbo.reports_Sales_Recognition_Detail_Global
WHERE 
		License_ID = @LicID
AND
		[Recognition Amount] > 0
AND
		[Recognition Unit] > 0		
GROUP BY
		license
		,Agent
					
UNION ALL

SELECT
		1 AS Ranking
		--,NULL AS License
		,NULL AS Agent
		,SUM([Recognition Amount]) AS Recognition_Unit			
FROM 
		dbo.reports_Sales_Recognition_Detail_Global	
WHERE License_ID = @LicID			
END
