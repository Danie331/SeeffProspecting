-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spMIS_RA_Region_Month_Unit] 
	-- Add the parameters for the stored procedure here
	@Region NVARCHAR(50) = 'Southern'
	,@Month INT = 4
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here	
     
	SELECT
		RANK() OVER (PARTITION BY Month ORDER BY SUM([Recognition Unit]) DESC) AS Ranking
		,License
		,Agent
		,SUM([Recognition Unit]) AS [Recognition Unit]	
FROM 
		dbo.reports_Sales_Recognition_Detail_Global
WHERE
		Region = @Region		
AND
		MONTH = @Month
GROUP BY 
		[MONTH]
		,License
		,Agent

UNION ALL

SELECT
		1 AS Ranking
		,'' AS License
		,NULL AS Agent
		,SUM([Recognition Unit]) AS Recognition		
FROM 
		dbo.reports_Sales_Recognition_Detail_Global
WHERE
		Region = @Region
AND
		MONTH = @Month
GROUP BY
		MONTH			
END
