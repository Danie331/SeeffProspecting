-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spMIS_Referral_Recognition_Region] 
	-- Add the parameters for the stored procedure here
	@region VARCHAR(50)  
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
		RANK() OVER (PARTITION BY month_reported ORDER BY SUM([Recognition_Unit]) DESC) AS Ranking
		,license_name AS [License]
		,Agent_Partnership AS [Agent]
		,SUM([Recognition_Unit]) AS Recognition	
FROM 
		dbo.reports_Referral_Recognition_Detail
WHERE
		region = @region
AND
		[Agent_Count] = @PartnerCount
AND
		transaction_division = @TranDiv
AND
		month_reported = @Month
GROUP BY 
		[month_reported]
		,license_name
		,Agent_Partnership

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
AND
		transaction_division = @TranDiv
AND
		month_reported = @Month
GROUP BY
		month_reported
	
END
