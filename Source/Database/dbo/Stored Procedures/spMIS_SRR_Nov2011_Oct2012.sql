
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spMIS_SRR_Nov2011_Oct2012] 
	-- Add the parameters for the stored procedure here

@LicenseID INT
,@Year01 INT = 2011
,@Year02 INT = 2012
--@smYear01 INT = RIGHT(@yEAR01,2)
--@smYear02 INT = RIGHT(@yEAR02,2)
--@strSQL NVARCHAR(MAX)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	CREATE TABLE #SRR_Results

(
	 LicenseID INT
	,Region NVARCHAR(MAX)	
	,License NVARCHAR(MAX)
	,[Total] DECIMAL (16,2)
	,[Total at 7%] DECIMAL (16,2)
	,[Nov '11] DECIMAL (16,2)
	,[Nov '11 at 7%] DECIMAL (16,2) 	
	,[Dec '11] DECIMAL (16,2)
	,[Dec '11 at 7%] DECIMAL (16,2)
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

)

INSERT #SRR_Results 
	(
	LicenseID
	,Region
	,License
	,[Nov '11]
	,[Nov '11 at 7%]
	,[Dec '11]
	,[Dec '11 at 7%]
	
	)	
(
SELECT		
			 RLS.license_id
			,license.region as [Region]	
			, license.license_name AS [License Name]
			, SUM(RLS.nov) AS 'Nov'
			, (SUM(RLS.nov)/0.07) AS [Nov @ 7%]
			, SUM(RLS.dec) AS 'Dec'
			, (SUM(RLS.dec)/0.07) AS [Dec @ 7%]
FROM         
			reports_license_summary AS RLS 
INNER JOIN
			license ON RLS.license_id = license.license_id
WHERE 
			(RLS.measurement_desc LIKE 'Monthly Rental Total')
AND
			(year = @Year01)
			
AND
			(RLS.license_id = @LicenseID)			
									
GROUP BY
			RLS.license_id
			,license.region
			,license.license_name
			,RLS.year
			,RLS.measurement_desc

UNION

SELECT	
					license.license_id
					,license.region
					,license.license_name
					,0
					,0
					,0
					,0
FROM license
where (license.license_id NOT IN (SELECT RLS.license_id FROM reports_license_summary RLS where (RLS.year <> @Year02)))
AND (license.Status like 'A')
AND (license.license_id = @LicenseID)
)

UPDatE #SRR_Results 
		SET [Jan '12] = (SELECT 
								SUM(RLS.jan) 
							FROM 
								reports_license_summary RLS
							WHERE 
								(RLS.measurement_desc LIKE 'Monthly Rental Total')
							AND
								(year = @Year02)
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
							(year = @Year02)
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
					(year = @Year02)
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
							(year = @Year02)
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
					(year = @Year02)
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
							(year = @Year02)
					 AND 
							(#SRR_Results.LicenseID = RLS.license_id)													
					)

,[Apr '12] = (SELECT 
								SUM(RLS.apr) 
							FROM 
								reports_license_summary RLS
							WHERE 
								(RLS.measurement_desc LIKE 'Monthly Rental Total')
							AND
								(year = @Year02)
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
							(year = @Year02)
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
								(year = @Year02)
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
							(year = @Year02)
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
					(year = @Year02)
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
							(year = @Year02)
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
					(year = @Year02)
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
							(year = @Year02)
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
					(year = @Year02)
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
							(year = @Year02)
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
					(year = @Year02)
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
							(year = @Year02)
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
						(year = @Year02)
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
							(year = @Year02)
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

, [Total at 7%]	= (
					[Nov '11 at 7%] +
					[Dec '11 at 7%] +
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
					
					

select * FROM #SRR_Results WHERE LicenseID = @LicenseID

DROP TABLE #SRR_Results
END

