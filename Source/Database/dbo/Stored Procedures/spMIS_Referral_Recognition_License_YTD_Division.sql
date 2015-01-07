-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spMIS_Referral_Recognition_License_YTD_Division] 
	-- Add the parameters for the stored procedure here
	@LicID INT = 93
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
		,Agent_Partnership AS [Agent]
		,SUM([Recognition_Unit]) AS Recognition
		,@PartnerCount AS [Partner Count]
		
FROM 
		dbo.reports_Referral_Recognition_Detail
WHERE
		license_id = @LicID
AND
		[Agent_Count] = @PartnerCount

GROUP BY
		Agent_Partnership
		,[Agent_Count]	
	
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
END
