-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spMIS_Referral_Recognition_License] 
	-- Add the parameters for the stored procedure here
	@LicID INT
	,@PartnerCount INT
	--,@TranDiv NVARCHAR(50)
	,@Month INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here	
     
	SELECT
		RANK() OVER (PARTITION BY month_reported ORDER BY SUM([Recognition_Unit]) DESC) AS Ranking
		,Agent_Partnership AS [Agent]
		,SUM([Recognition_Unit]) AS Recognition
		,@PartnerCount AS [Partner Count]
		
FROM 
		dbo.reports_Referral_Recognition_Detail
WHERE
		license_id = @LicID
AND
		[Agent_Count] = @PartnerCount
--AND
--		[transaction_division] = @TranDiv
AND
		month_reported = @Month
GROUP BY 
		month_reported
		,Agent_Partnership

--works till here

UNION ALL

SELECT
		1 AS Ranking
		,NULL AS Agent
		,SUM([Recognition_Unit]) AS Recognition
		,@PartnerCount AS [Partner Count]
		
FROM 
		dbo.reports_Referral_Recognition_Detail
WHERE
		license_id = @LicID
AND
		[Agent_Count] = @PartnerCount
--AND
--		[transaction_division] = @TranDiv
AND
		month_reported = @Month
GROUP BY
		month_reported	
			
END
