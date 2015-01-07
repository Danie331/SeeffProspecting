-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spMIS_Sales_Recognition_License_YTD_Division] 
	-- Add the parameters for the stored procedure here
	@LicID INT
	,@PartnerCount INT
	,@TranDiv NVARCHAR(50)
	--,@Month INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here	
     
	SELECT
		RANK() OVER (PARTITION BY [SPS Transaction Division] ORDER BY SUM([Recognition Unit]) DESC) AS Ranking
		,Agent
		,SUM([Recognition Unit]) AS Recognition
		,@PartnerCount AS [Partner Count]
		
FROM 
		dbo.reports_Sales_Recognition_Detail
WHERE
		LICENSE_ID = @LicID
AND
		[Partner Count] = @PartnerCount
AND
		[SPS Transaction Division] = @TranDiv
GROUP BY
		[SPS Transaction Division]
		,Agent		
	
UNION ALL

SELECT
		1 AS Ranking
		,NULL AS Agent
		,SUM([Recognition Unit]) AS Recognition
		,@PartnerCount AS [Partner Count]
		
FROM 
		dbo.reports_Sales_Recognition_Detail
WHERE
		LICENSE_ID = @LicID
AND
		[Partner Count] = @PartnerCount
AND
		[SPS Transaction Division] = @TranDiv
GROUP BY
		[SPS Transaction Division]

UNION ALL

SELECT
		RANK() OVER (PARTITION BY [SPS Transaction Division] ORDER BY SUM([Recognition Amount]) DESC) AS Ranking
		,Agent
		,SUM([Recognition Amount]) AS Recognition_Amount_Total
		,@PartnerCount AS [Partner Count]	
FROM 
		dbo.reports_Sales_Recognition_Detail
WHERE
		LICENSE_ID = @LicID
AND
		[Partner Count] = @PartnerCount
AND
		[SPS Transaction Division] = @TranDiv			
GROUP BY 
		[SPS Transaction Division]
		,Agent	

UNION ALL

SELECT
		1 AS Ranking
		,NULL AS Agent
		,SUM([Recognition Amount]) AS Recognition
		,@PartnerCount AS [Partner Count]
		
FROM 
		dbo.reports_Sales_Recognition_Detail
WHERE
		LICENSE_ID = @LicID
AND
		[Partner Count] = @PartnerCount
AND
		[SPS Transaction Division] = @TranDiv
GROUP BY
		[SPS Transaction Division]			
END
