
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spMIS_Sales_Recognition_Region] 
	-- Add the parameters for the stored procedure here
	@Region VARCHAR(50)  
	,@PartnerCount INT 
	,@TranDiv NVARCHAR(50) 
	,@Month INT 
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
		,SUM([Recognition Unit]) AS Recognition
		
		
FROM 
		dbo.reports_Sales_Recognition_Detail
WHERE
		Region = @Region
AND
		[Partner Count] = @PartnerCount
AND
		[SPS Transaction Division] = @TranDiv
AND
		MONTH = @Month
GROUP BY 
		[MONTH]
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
		Region = @Region
AND
		[Partner Count] = @PartnerCount
AND
		[SPS Transaction Division] = @TranDiv
AND
		MONTH = @Month
GROUP BY
		MONTH
	

UNION ALL

SELECT
		RANK() OVER (PARTITION BY Month ORDER BY SUM([Recognition Amount]) DESC) AS Ranking
		,License
		,Agent
		,SUM([Recognition Amount]) AS Recognition_Amount_Total
		--,@PartnerCount AS [Partner Count]	
FROM 
		dbo.reports_Sales_Recognition_Detail
WHERE
		Region = @Region
AND
		[Partner Count] = @PartnerCount
AND
		[SPS Transaction Division] = @TranDiv			
AND
		MONTH = @Month
GROUP BY 
		[MONTH]
		,License
		,Agent

UNION ALL

SELECT
		1 AS Ranking
		,NULL AS License
		,NULL AS Agent
		,SUM([Recognition Amount]) AS Recognition
		--,@PartnerCount AS [Partner Count]
		
FROM 
		dbo.reports_Sales_Recognition_Detail
WHERE
		Region = @Region
AND
		[Partner Count] = @PartnerCount
AND
		[SPS Transaction Division] = @TranDiv
AND
		MONTH = @Month
GROUP BY
		MONTH	
				
END
