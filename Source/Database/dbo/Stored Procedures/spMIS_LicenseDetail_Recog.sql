
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spMIS_LicenseDetail_Recog] 
	-- Add the parameters for the stored procedure here
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	
		

    -- Insert statements for procedure here
	SELECT DISTINCT
		lic.license_id as [License_ID]
		,Region = CASE LIC.region
		WHEN 'Free State' THEN 'Northern'
		WHEN 'Northern Region' THEN 'Northern'
		ELSE LIC.region 
		END
		,lic.license_name
		,lic.license_name + ' - ' + lic.Status AS License_Name_Status
INTO 
		#LicenseDetailRecog			
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
		
		ALTER TABLE #LicenseDetailRecog
			ADD Division NVARCHAR(MAX)

DECLARE @intYear INT
SET @intYear = (SELECT DATEPART(YYYY,GETDATE()))
DECLARE @DivisionType NVARCHAR(MAX)
SET @DivisionType = ''

DECLARE @LicID INT

DECLARE curLicID CURSOR
	FOR SELECT #LicenseDetailRecog.License_ID FROM #LicenseDetailRecog
	
	OPEN curLicID
		FETCH NEXT FROM curLicID
		INTO @LicID;
		
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
										license ON license_branches.license_id = @LicID
								 WHERE
										DATEPART(YEAR,sps_transaction.sps_reporting_date) = @intYear
										) sps_tran
												
	UPDATE #LicenseDetailRecog
		SET Division = @DivisionType WHERE #LicenseDetailRecog.License_ID = @LicID
		
		SET @DivisionType = ''
		FETCH NEXT FROM curLicID INTO @LicID
		END
		CLOSE curLicID
		DEALLOCATE curLicID
										
SELECT * FROM #LicenseDetailRecog 
WHERE Division <> ''
DROP TABLE #LicenseDetailRecog	
END
