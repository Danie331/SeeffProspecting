

-- =============================================
-- Author:		Gustav Swanepoel
-- Create date: 2012-01-06
-- Description:	Returns License information
--				to be used for report creation
-- =============================================
CREATE PROCEDURE [dbo].[spMIS_Active_License_Detail] 
	-- Add the parameters for the stored procedure here

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT 
		lic.license_id as [License_ID]
		,Region = CASE LIC.region
		WHEN 'Free State' THEN 'Northern'
		WHEN 'Northern Region' THEN 'Northern'
		ELSE LIC.region 
		END
		,lic.license_name
		,lic.license_name + ' - ' + lic.Status AS License_Name_Status

FROM
		license lic
WHERE
		(lic.license_id NOT IN (107,109)	)
AND
		lic.Status NOT LIKE 'C'		
		
ORDER BY
		lic.region,
		[License_Name_Status] ASC
END




