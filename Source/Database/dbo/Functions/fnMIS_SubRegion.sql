-- =============================================
-- Author:	GW Swanepoel
-- Create date: 2013-02-09	
-- Description:	Returns 'subregions' for reports
--		packs.
--		Gauteng
--		Central Reg
--		Commercial
--		KZN
--		Lim / Mpu
--		Country
--		Eastern Cape
--		Garden Route
-- =============================================
CREATE FUNCTION dbo.fnMIS_SubRegion
(
	-- Add the parameters for the function here
	@license_ID INT 
)
RETURNS NVARCHAR(MAX)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @returnValue NVARCHAR(MAX)

SET @returnValue = (
					SELECT
					CASE
						WHEN @license_ID IN (21,53,58) THEN 'Gauteng'	 
						WHEN @license_ID IN (1,2,3,52,56) THEN 'Central Reg'
						WHEN @license_ID IN (73,100,110) THEN 'Commercial'
						WHEN @license_ID IN (42,43,44,48,49,50,51,111) THEN 'Lim / Mpu'	
					ELSE
						license.sub_region
					END
					FROM
							license
					WHERE license.license_id = @license_id				
					)
BEGIN
	IF @license_id NOT IN (SELECT license.license_ID FROM license)
		SET @returnValue = 'Error'	
END

BEGIN
	IF (@license_ID IN (SELECT license.license_id FROM license WHERE (license.region LIKE 'KZN') AND (license.license_ID = @license_id) ))
			SET @returnValue = (SELECT license.region from license WHERE license.license_id = @license_ID)
END

BEGIN
	IF (@license_ID IN (SELECT license.license_id FROM license WHERE (license.region LIKE 'International') AND (license.license_ID = @license_id) ))
		SET @returnValue = (SELECT license.region from license WHERE license.license_id = @license_ID)
END

	-- Return the result of the function
	RETURN @returnValue

END
