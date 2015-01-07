
-- =============================================
-- Author:		GW Swanepoel
-- Create date: 2012-12-03
-- Description:	Returns the totals for
--				Units and Rental Recog amounts
--				per month
--				Used in report:
--				Rental_Recognition.xlsm
-- =============================================
CREATE PROCEDURE [dbo].[spMIS_RentRecog_Totals_PerMonth] 
	-- Add the parameters for the stored procedure here
	@Year_Start INT
	,@Month_Start INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT 
			'Total Recognition Amount: ' 
			,ISNULL(SUM([comm]),0) AS [Total]
			
	  FROM 
		[boss].[dbo].[reports_rental_recognition] AS RRR
INNER JOIN
		license ON RRR.license_id = license.license_id		
WHERE 
--		license.region = @strRegion
--AND
		(YEAR(CONTRACT_START_DATE) = @Year_Start)
AND
		(MONTH(CONTRACT_START_DATE) = @Month_Start)
				
UNION

SELECT 
			'Total Units: ' 
			, SUM([units]) AS [Total]
	  FROM 
		[boss].[dbo].[reports_rental_recognition] AS RRR
INNER JOIN
		license ON RRR.license_id = license.license_id		
WHERE 
		(YEAR(CONTRACT_START_DATE) = @Year_Start)
AND
		(MONTH(CONTRACT_START_DATE) = @Month_Start)
END

