
-- =============================================
-- Author:		GW Swanepoel
-- Create date: 2012-11-16
-- Description:	Recordset for running
--				the Rental and Other Income
--				verificvation letters.
-- =============================================
CREATE PROCEDURE [dbo].[spMIS_Rental_OI_Verification_Letter] 
	-- Add the parameters for the stored procedure here

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
--DECLARE @Year01 INT = 2011
--DECLARE @Year02 INT = 2012

CREATE TABLE #SRR_Results

(
LicenseID INT
,OwnerName NVARCHAR(MAX)
,Owner NVARCHAR(MAX) 
,Region NVARCHAR(MAX)
,License_Status NVARCHAR(MAX)	
,License NVARCHAR(MAX)
,[Total] DECIMAL (16,2)
,[Total @ 7%] DECIMAL (16,2)
,[Nov '11] DECIMAL (16,2)
,[Nov '11 @ 7%] DECIMAL (16,2) 	
,[Dec '11] DECIMAL (16,2)
,[Dec '11 @ 7%] DECIMAL (16,2)
,[Jan '12] DECIMAL (16,2)
,[Jan '12 at 7%] DECIMAL (16,2) 	
,[Feb '12] DECIMAL (16,2)
,[Feb '12 at 7%] DECIMAL (16,2)
,[Mar '12] DECIMAL (16,2)
,[Mar '12 at 7%] DECIMAL (16,2)
,[Apr '12] DECIMAL (16,2)
,[Apr '12 at 7%] DECIMAL (16,2)
,[May '12] DECIMAL (16,2)
,[May '12 at 7%] DECIMAL (16,2)
,[Jun '12] DECIMAL (16,2)
,[Jun '12 at 7%] DECIMAL (16,2)
,[Jul '12] DECIMAL (16,2)
,[Jul '12 at 7%] DECIMAL (16,2)
,[Aug '12] DECIMAL (16,2)
,[Aug '12 at 7%] DECIMAL (16,2)
,[Sep '12] DECIMAL (16,2)
,[Sep '12 at 7%] DECIMAL (16,2)
,[Oct '12] DECIMAL (16,2)
,[Oct '12 at 7%] DECIMAL (16,2)
,[OIatSvn2011] DECIMAL (16,2)
,[OIatSvn2012] DECIMAL (16,2)

)

INSERT #SRR_Results 
(
LicenseID
,OwnerName
,Owner 
,Region
,License_Status
,License
,[Nov '11]
,[Nov '11 @ 7%]
,[Dec '11]
,[Dec '11 @ 7%]

)	
(
SELECT		
RLS.license_id
, user_registration.user_name
, user_registration.user_name + ' ' + user_registration.user_surname
, license.region as [Region]
, license.license_name + ' - ' + license.Status	
, license.license_name AS [License Name]
, SUM(RLS.nov) AS 'Nov'
, (SUM(RLS.nov)/0.07) AS [Nov @ 7%]
, SUM(RLS.dec) AS 'Dec'
, (SUM(RLS.dec)/0.07) AS [Dec @ 7%]
FROM         
reports_license_summary AS RLS 
INNER JOIN
license ON RLS.license_id = license.license_id
--INNER Join license_branches ON sps_transaction.branch_id = license_branches.branch_id
--INNER Join license ON license_branches.license_id = license.license_id
INNER Join licensee ON license.license_id = licensee.license_id
INNER Join user_registration ON licensee.registration_id = user_registration.registration_id					
WHERE 
(RLS.measurement_desc LIKE 'Monthly Rental Total')
AND
(year = 2011)

--AND
--			(RLS.license_id = @LicenseID)			

GROUP BY
RLS.license_id
,license.region
,license.license_name
,Status
,RLS.year
,RLS.measurement_desc
,user_registration.user_name
,user_registration.user_name + ' ' + user_registration.user_surname

UNION

SELECT	
license.license_id
,user_registration.user_name
,user_registration.user_name + ' ' + user_registration.user_surname
,license.region
,license.license_name + ' - ' + license.Status	
,license.license_name
,0
,0
,0
,0
FROM license
INNER Join licensee ON license.license_id = licensee.license_id
INNER Join user_registration ON licensee.registration_id = user_registration.registration_id
where (license.license_id NOT IN (SELECT RLS.license_id FROM reports_license_summary RLS where (RLS.year <> 2012)))
AND (license.Status like 'A')
--AND (license.license_id = @LicenseID)
)

UPDatE #SRR_Results 
SET [Jan '12] = (SELECT 
SUM(RLS.jan) 
FROM 
reports_license_summary RLS
WHERE 
(RLS.measurement_desc LIKE 'Monthly Rental Total')
AND
(year = 2012)
AND 
(#SRR_Results.LicenseID = RLS.license_id)			
)

,[Jan '12 at 7%] = (SELECT 
(SUM(RLS.jan)/0.07) 
FROM         
reports_license_summary RLS 		
WHERE 
(RLS.measurement_desc LIKE 'Monthly Rental Total')
AND
(year = 2012)
AND 
(#SRR_Results.LicenseID = RLS.license_id)													
)

,[Feb '12] = (SELECT 
SUM(RLS.feb) 
FROM 
reports_license_summary RLS
WHERE 
(RLS.measurement_desc LIKE 'Monthly Rental Total')
AND
(year = 2012)
AND 
(#SRR_Results.LicenseID = RLS.license_id)			
)

,[Feb '12 at 7%] = (SELECT 
(SUM(RLS.feb)/0.07) 
FROM         
reports_license_summary RLS 		
WHERE 
(RLS.measurement_desc LIKE 'Monthly Rental Total')
AND
(year = 2012)
AND 
(#SRR_Results.LicenseID = RLS.license_id)													
)					

,[Mar '12] = (SELECT 
SUM(RLS.mar) 
FROM 
reports_license_summary RLS
WHERE 
(RLS.measurement_desc LIKE 'Monthly Rental Total')
AND
(year = 2012)
AND 
(#SRR_Results.LicenseID = RLS.license_id)			
)

,[Mar '12 at 7%] = (SELECT 
(SUM(RLS.mar)/0.07) 
FROM         
reports_license_summary RLS 		
WHERE 
(RLS.measurement_desc LIKE 'Monthly Rental Total')
AND
(year = 2012)
AND 
(#SRR_Results.LicenseID = RLS.license_id)													
)

,[Apr '12] = (SELECT 
SUM(RLS.mar) 
FROM 
reports_license_summary RLS
WHERE 
(RLS.measurement_desc LIKE 'Monthly Rental Total')
AND
(year = 2012)
AND 
(#SRR_Results.LicenseID = RLS.license_id)			
)

,[Apr '12 at 7%] = (SELECT 
(SUM(RLS.apr)/0.07) 
FROM         
reports_license_summary RLS 		
WHERE 
(RLS.measurement_desc LIKE 'Monthly Rental Total')
AND
(year = 2012)
AND 
(#SRR_Results.LicenseID = RLS.license_id)													
)

,[May '12] = (SELECT 
SUM(RLS.may) 
FROM 
reports_license_summary RLS
WHERE 
(RLS.measurement_desc LIKE 'Monthly Rental Total')
AND
(year = 2012)
AND 
(#SRR_Results.LicenseID = RLS.license_id)			
)

,[May '12 at 7%] = (SELECT 
(SUM(RLS.may)/0.07) 
FROM         
reports_license_summary RLS 		
WHERE 
(RLS.measurement_desc LIKE 'Monthly Rental Total')
AND
(year = 2012)
AND 
(#SRR_Results.LicenseID = RLS.license_id)													
)

,[Jun '12] = (SELECT 
SUM(RLS.jun) 
FROM 
reports_license_summary RLS
WHERE 
(RLS.measurement_desc LIKE 'Monthly Rental Total')
AND
(year = 2012)
AND 
(#SRR_Results.LicenseID = RLS.license_id)			
)

,[Jun '12 at 7%] = (SELECT 
(SUM(RLS.jun)/0.07) 
FROM         
reports_license_summary RLS 		
WHERE 
(RLS.measurement_desc LIKE 'Monthly Rental Total')
AND
(year = 2012)
AND 
(#SRR_Results.LicenseID = RLS.license_id)													
)

,[Jul '12] = (SELECT 
SUM(RLS.jul) 
FROM 
reports_license_summary RLS
WHERE 
(RLS.measurement_desc LIKE 'Monthly Rental Total')
AND
(year = 2012)
AND 
(#SRR_Results.LicenseID = RLS.license_id)			
)

,[Jul '12 at 7%] = (SELECT 
(SUM(RLS.jul)/0.07) 
FROM         
reports_license_summary RLS 		
WHERE 
(RLS.measurement_desc LIKE 'Monthly Rental Total')
AND
(year = 2012)
AND 
(#SRR_Results.LicenseID = RLS.license_id)													
)

,[Aug '12] = (SELECT 
SUM(RLS.aug) 
FROM 
reports_license_summary RLS
WHERE 
(RLS.measurement_desc LIKE 'Monthly Rental Total')
AND
(year = 2012)
AND 
(#SRR_Results.LicenseID = RLS.license_id)			
)

,[Aug '12 at 7%] = (SELECT 
(SUM(RLS.aug)/0.07) 
FROM         
reports_license_summary RLS 		
WHERE 
(RLS.measurement_desc LIKE 'Monthly Rental Total')
AND
(year = 2012)
AND 
(#SRR_Results.LicenseID = RLS.license_id)													
)

,[Sep '12]  = (SELECT 
SUM(RLS.sep) 
FROM 
reports_license_summary RLS
WHERE 
(RLS.measurement_desc LIKE 'Monthly Rental Total')
AND
(year = 2012)
AND 
(#SRR_Results.LicenseID = RLS.license_id)			
)

,[Sep '12 at 7%] = (SELECT 
(SUM(RLS.sep)/0.07) 
FROM         
reports_license_summary RLS 		
WHERE 
(RLS.measurement_desc LIKE 'Monthly Rental Total')
AND
(year = 2012)
AND 
(#SRR_Results.LicenseID = RLS.license_id)													
)

,[Oct '12]  = (SELECT 
SUM(RLS.oct) 
FROM 
reports_license_summary RLS
WHERE 
(RLS.measurement_desc LIKE 'Monthly Rental Total')
AND
(year = 2012)
AND 
(#SRR_Results.LicenseID = RLS.license_id)			
)

,[Oct '12 at 7%] = (SELECT 
(SUM(RLS.oct)/0.07) 
FROM         
reports_license_summary RLS 		
WHERE 
(RLS.measurement_desc LIKE 'Monthly Rental Total')
AND
(year = 2012)
AND 
(#SRR_Results.LicenseID = RLS.license_id)													
)

UPDATE #SRR_Results
SET [Total] = ([Nov '11] 
+ [Dec '11] 
+ [Jan '12] 
+ [Feb '12] 
+ [Mar '12] 
+ [Apr '12] 
+ [May '12] 
+ [Jun '12]
+ [Jul '12]
+ [Aug '12]
+ [Sep '12]
+ [Oct '12]
)

, [Total @ 7%]	= (
[Nov '11 @ 7%] +
[Dec '11 @ 7%] +
[Jan '12 at 7%] +
[Feb '12 at 7%] +
[Mar '12 at 7%] +
[Apr '12 at 7%] +
[May '12 at 7%] +
[Jun '12 at 7%] +
[Jul '12 at 7%] +
[Aug '12 at 7%] +
[Sep '12 at 7%] +
[Oct '12 at 7%]
)				

UPDATE #SRR_Results 
SET 
[OIatSvn2011] = ( 
SELECT 
SUM(sps_oi.reported_amount/0.07) AS [OIatSeven2011] 
FROM 
sps_other_income AS sps_oi
WHERE
(sps_oi.reporting_year = 2011)
AND
(sps_oi.reporting_month >= 11) 
AND 
(sps_oi.reporting_month <= 12)
AND 
(sps_oi.license_id = #SRR_Results.LicenseID )
)
,[OIatSvn2012] = ( 
SELECT 
SUM(sps_oi.reported_amount/0.07) AS [OIatSeven2011] 
FROM 
sps_other_income AS sps_oi
WHERE
(sps_oi.reporting_year = 2012)
AND
(sps_oi.reporting_month >= 1) 
AND 
(sps_oi.reporting_month <= 10)
AND 
(sps_oi.license_id = #SRR_Results.LicenseID )
)

SELECT 
LicenseID
,Owner
,OwnerName
, Region
, License
, License_Status
, ISNULL([Total @ 7%],0) AS [SSR]
, ISNULL([OIatSvn2011],0) + ISNULL([OIatSvn2012],0) AS [OtherIncome]
FROM 
#SRR_Results 
WHERE 
(LicenseID NOT IN (2,7,86,87,89,99,100,109,115,107,42,73))


DROP TABLE #SRR_Results
END

