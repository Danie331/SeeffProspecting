-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE spMIS_IncProj_OI_2011_2012 
	-- Add the parameters for the stored procedure here
	@Year01 INT
	,@Year02 INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT     
		license.license_name AS [License]
		,CONVERT(varchar, SPS_OI.reporting_year) 
		+ '-' + CONVERT(varchar, SPS_OI.reporting_month) AS [Year Month Reported]
		,SPS_OI.reported_amount AS [Reported Amount]
		,(SPS_OI.reported_amount/0.07) AS [Reported Amount @ 7%] 
		,SPS_OI.description AS [Description]
		,UR.user_preferred_name + ' ' + UR.user_surname AS [Reported By]
		,UR.user_email_address AS [User Email Address]
FROM         
			sps_other_income SPS_OI 
INNER JOIN
			license ON SPS_OI.license_id = license.license_id 
INNER JOIN
			user_registration UR ON SPS_OI.reported_by = UR.registration_id
WHERE 
--			license.license_id = @LicenseID
--AND
			SPS_OI.reporting_year = @Year01
AND
			SPS_OI.reporting_month >= 11
AND
			SPS_OI.reporting_month <= 12
			
UNION

SELECT
			license.license_name
			,'2011'
			,0
			,0
			,'No reported other income for 2011.'
			,''
			,''
FROM
			license
WHERE
			license.license_id NOT IN (
										SELECT     
												SPS_OI.license_id
										FROM         
													sps_other_income SPS_OI 
										INNER JOIN
													license ON SPS_OI.license_id = license.license_id 
										INNER JOIN
													user_registration UR ON SPS_OI.reported_by = UR.registration_id
										WHERE 
										--			license.license_id = @LicenseID
										--AND
													SPS_OI.reporting_year = @Year01
										AND
													SPS_OI.reporting_month >= 11
										AND
													SPS_OI.reporting_month <= 12
									   )
	AND license.Status LIKE 'A'

UNION


SELECT     
		license.license_name AS [License]
		,CONVERT(varchar, SPS_OI.reporting_year) 
		+ '-' + CONVERT(varchar, SPS_OI.reporting_month) AS [Year Month Reported]
		,SPS_OI.reported_amount AS [Reported Amount]
		,(SPS_OI.reported_amount/0.07) AS [Reported Amount @ 7%] 
		,SPS_OI.description AS [Description]
		,UR.user_preferred_name + ' ' + UR.user_surname AS [Reported By]
		,UR.user_email_address AS [User Email Address]
FROM         
			sps_other_income SPS_OI 
INNER JOIN
			license ON SPS_OI.license_id = license.license_id 
INNER JOIN
			user_registration UR ON SPS_OI.reported_by = UR.registration_id
WHERE 
--			license.license_id = @LicenseID
--AND
			SPS_OI.reporting_year = @Year02
AND
			SPS_OI.reporting_month >= 1
AND
			SPS_OI.reporting_month <= 10
			
UNION

SELECT
			license.license_name
			,'2012'
			,0
			,0
			,'No reported other income for 2012.'
			,''
			,''
FROM
			license
WHERE
			license.license_id NOT IN (
										SELECT     
												SPS_OI.license_id
										FROM         
													sps_other_income SPS_OI 
										INNER JOIN
													license ON SPS_OI.license_id = license.license_id 
										INNER JOIN
													user_registration UR ON SPS_OI.reported_by = UR.registration_id
										WHERE 
										--			license.license_id = @LicenseID
										--AND
													SPS_OI.reporting_year = @Year02
										AND
													SPS_OI.reporting_month >= 1
										AND
													SPS_OI.reporting_month <= 10
									   )
	AND license.Status LIKE 'A'	
END
