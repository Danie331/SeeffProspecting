-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spMIS_RA_License_Month_Rand] 
	-- Add the parameters for the stored procedure here
	@LicID INT = 89
	,@Month INT = 4
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here	
     
	SELECT
		RANK() OVER (PARTITION BY Month ORDER BY SUM([Recognition Amount]) DESC) AS Ranking
		,Agent
		,SUM([Recognition Amount]) AS Recognition_Amount_Total	
FROM 
		dbo.reports_Sales_Recognition_Detail_Global
WHERE
		LICENSE_ID = @LicID		
AND
		MONTH = @Month
AND
		[Recognition Amount] > 0
AND
		[Recognition Unit] > 0		
GROUP BY 
		[MONTH]
		,Agent

UNION ALL

SELECT
		1 AS Ranking
		,NULL AS Agent
		,SUM([Recognition Amount]) AS Recognition		
FROM 
		dbo.reports_Sales_Recognition_Detail_Global
WHERE
		LICENSE_ID = @LicID
AND
		MONTH = @Month
GROUP BY
		MONTH			
END
