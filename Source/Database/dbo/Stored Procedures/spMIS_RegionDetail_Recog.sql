

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spMIS_RegionDetail_Recog] 
	-- Add the parameters for the stored procedure here
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT DISTINCT
		--lic.license_id as [License_ID]
		Region = CASE LIC.region
		WHEN 'Free State' THEN 'Northern'
		WHEN 'Northern Region' THEN 'Northern'
		ELSE LIC.region 
		END
	
INTO 
		#RegionDetailRecog			
FROM
		license lic 
LEFT JOIN
		license_branches ON lic.license_id = license_branches.license_id
LEFT JOIN
		branch on license_branches.branch_id = branch.branchId
LEFT JOIN
		SPS_TRANSACTION on branch.branchId = sps_transaction.branch_id 			
WHERE
		lic.license_id NOT IN (107,109)
AND
		lic.Status = 'A'		
		
		ALTER TABLE #RegionDetailRecog
			ADD Division NVARCHAR(MAX)

DECLARE @intMonth INT 
SET @intMonth= DATEPART(MONTH,GETDATE())

DECLARE @intYear INT 
SET @intYear= DATEPART(YEAR,GETDATE())

IF @intMonth = 1
BEGIN
  SET @intYear = (@intYear - 1)
PRINT convert(varchar, @intYear)
END

DECLARE @DivisionType NVARCHAR(MAX)
SET @DivisionType = ''

DECLARE @Region NVARCHAR(50)

DECLARE curRegion CURSOR
	FOR SELECT #RegionDetailRecog.Region FROM #RegionDetailRecog
	
	OPEN curRegion
		FETCH NEXT FROM curRegion
		INTO @Region;
		
		WHILE @@FETCH_STATUS = 0
			BEGIN
				SELECT  
					@DivisionType = @DivisionType + '|' +  sps_transaction_division 
					   FROM 
								(SELECT 
										DISTINCT sps_transaction.sps_transaction_division 
								 FROM 
										sps_transaction 
								 JOIN
										branch ON sps_transaction.branch_id = branch.branchId
								 JOIN
										license_branches ON branch.branchId = license_branches.branch_id
								 JOIN
										license ON license_branches.license_id = license.license_id
								 WHERE
										DATEPART(YEAR,sps_transaction.sps_reporting_date) = @intYear
								AND
										license.region = @Region
										) sps_tran
										
												
	UPDATE #RegionDetailRecog
		SET Division = @DivisionType WHERE #RegionDetailRecog.Region = @Region
		
		SET @DivisionType = ''
		FETCH NEXT FROM curRegion INTO @Region
		END
		CLOSE curRegion
		DEALLOCATE curRegion
										
SELECT * FROM #RegionDetailRecog 
WHERE Division <> ''
DROP TABLE #RegionDetailRecog	
END

