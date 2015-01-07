-- =============================================
-- Author:		GW Swanepoel
-- Create date: 2013-02-04
-- Description:	Returns license_id's
--				where a Sales ReportSubRegion 
--				has not been allocated
-- =============================================
CREATE PROCEDURE [dbo].[spMIS_Reports_SubRegion_Error] 
	-- Add the parameters for the stored procedure here
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	
DECLARE @returnValue NVARCHAR(MAX)

CREATE TABLE #MIS_SubRegion (license_ID INT, SubRegion NVARCHAR(MAX))

INSERT #MIS_SubRegion
SELECT
		license.license_id AS license_id
		,license.sub_region AS [SubRegion]
FROM
		license
				
UPDATE #MIS_SubRegion
	SET SubRegion = 'Gauteng' WHERE #MIS_SubRegion.license_id IN (21,53,58)	

UPDATE #MIS_SubRegion
	SET SubRegion = 'Commercial' WHERE #MIS_SubRegion.license_id IN (73,100,110)

UPDATE #MIS_SubRegion
	SET SubRegion = 'Lim / Mpu' WHERE #MIS_SubRegion.license_id IN (42,43,44,48,49,50,51,111)			
	
UPDATE #MIS_SubRegion
	SET SubRegion = 'Central Reg' WHERE #MIS_SubRegion.license_id IN (1,2,3,52,56)

UPDATE #MIS_SubRegion
	SET SubRegion = 'Error' WHERE SubRegion IS NULL	

SELECT * FROM #MIS_SubRegion WHERE SubRegion like 'Error'
--WHERE SubRegion LIKE 'Error'
DROP TABLE #MIS_SubRegion
END
