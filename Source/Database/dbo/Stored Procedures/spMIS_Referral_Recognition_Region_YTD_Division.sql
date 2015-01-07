-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spMIS_Referral_Recognition_Region_YTD_Division] 
	-- Add the parameters for the stored procedure here
	@region VARCHAR(50) = 'Northern'
	,@PartnerCount INT = 1
	--,@TranDiv NVARCHAR(50) = 'Residential'
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here	
     
	SELECT
		RANK() OVER (PARTITION BY [Agent_Count] ORDER BY SUM([Recognition_Unit]) DESC) AS Ranking
		,license_name AS [License]
		,Agent_Partnership AS [Agent]
		,SUM([Recognition_Unit]) AS Recognition		
FROM 
		dbo.reports_Referral_Recognition_Detail
WHERE
		region = @region
AND
		[Agent_Count] = @PartnerCount
GROUP BY
		transaction_division
		,license_name
		,Agent_Partnership
		,[Agent_Count]		
	
UNION ALL

SELECT
		1 AS Ranking
		,NULL AS license_name
		,NULL AS Agent_Partnership
		,SUM([Recognition_Unit]) AS Recognition	
FROM 
		dbo.reports_Referral_Recognition_Detail
WHERE
		region = @region
AND
		[Agent_Count] = @PartnerCount
--AND
--		transaction_division = @TranDiv
--GROUP BY
--		transaction_division
		
END
