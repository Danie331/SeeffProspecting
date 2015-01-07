
-- =============================================
-- Author:		GW Swanepoel
-- Create date: 2012-06-27
-- Description:	License Key Indicators
--				Returns Key Indicators for
--				License Sales Report
-- =============================================
CREATE PROCEDURE [dbo].[spMIS_Lic_Key_Indicators]
	-- Add the parameters for the stored procedure here
	@YearFor NVARCHAR(4)
	,@License_ID NVARCHAR(10)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT 
		measurement_desc AS [Measurement Description]
		, year AS [Year]
		, CASE 
			WHEN measurement_desc LIKE 'Agents' THEN ''
			WHEN measurement_desc LIKE 'Sole Mandates' THEN ''
			ELSE CAST(SUM(total)AS NVARCHAR(MAX)) 
			END AS [Total]
		, SUM(jan) AS [Jan]
		, SUM(feb) AS [Feb]
		, SUM(mar) AS [Mar]
		, SUM(apr) AS [Apr]
		, SUM(may) AS [May]
		, SUM(jun) AS [Jun]
		, SUM(jul) AS [Jul]
		, SUM(aug) AS [Aug]
		, SUM(sep) AS [Sep]
		, SUM(oct) AS [Oct]
		, SUM(nov) AS [Nov]
		, SUM(dec) AS [Dec]
FROM 
		reports_license_summary
WHERE 
		(year = @YearFor)
AND 
		(license_id = @License_ID)
AND 
		(measurement_desc NOT LIKE 'Monthly Rental Total')
GROUP BY 
		measurement_desc
		, year
		, license_id
END


