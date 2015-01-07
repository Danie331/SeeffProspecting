

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spMIS_Sales_Recognition_National_YTD_Division] 
	-- Add the parameters for the stored procedure here

	@PartnerCount INT = 1
	,@TranDiv NVARCHAR(50) = 'Residential'

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here	
     
	SELECT
		RANK() OVER (PARTITION BY [SPS Transaction Division] ORDER BY SUM([Recognition Unit]) DESC) AS Ranking
		,License
		,Agent
		,SUM([Recognition Unit]) AS Recognition
		--,@PartnerCount AS [Partner Count]
		
FROM 
		dbo.reports_Sales_Recognition_Detail
WHERE
		[Partner Count] = @PartnerCount
AND
		[SPS Transaction Division] = @TranDiv
GROUP BY
		[SPS Transaction Division]
		,License
		,Agent		
	
UNION ALL

SELECT
		1 AS Ranking
		,NULL AS License
		,NULL AS Agent
		,SUM([Recognition Unit]) AS Recognition
				
FROM 
		dbo.reports_Sales_Recognition_Detail
WHERE
		[Partner Count] = @PartnerCount
AND
		[SPS Transaction Division] = @TranDiv
GROUP BY
		[SPS Transaction Division]

UNION ALL

SELECT
		RANK() OVER (PARTITION BY [SPS Transaction Division] ORDER BY SUM([Recognition Amount]) DESC) AS Ranking
		,License
		,Agent
		,SUM([Recognition Amount]) AS Recognition_Amount_Total
FROM 
		dbo.reports_Sales_Recognition_Detail
WHERE
		[Partner Count] = @PartnerCount
AND
		[SPS Transaction Division] = @TranDiv			
GROUP BY 
		[SPS Transaction Division]
		,License
		,Agent	

UNION ALL

SELECT
		1 AS Ranking
		,NULL as License
		,NULL AS Agent
		,SUM([Recognition Amount]) AS Recognition		
FROM 
		dbo.reports_Sales_Recognition_Detail
WHERE
		[Partner Count] = @PartnerCount
AND
		[SPS Transaction Division] = @TranDiv
GROUP BY
		[SPS Transaction Division]			
END


