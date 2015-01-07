


-- =============================================
-- Author:		GW Swanepoel
-- Create date: 2013-04-09
-- Description:	Key Indicators Report Automoted
--				for Karen G.
-- =============================================
CREATE PROCEDURE [dbo].[spMIS_KeyIndicators]
	-- Add the parameters for the stored procedure here
	@strRegion NVARCHAR(MAX) = 'ALL'
	,@intMonth INT = 3
	,@intYear NVARCHAR(MAX) = '2013'
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
DECLARE @strMonth NVARCHAR(MAX)
DECLARE @strSQL NVARCHAR(MAX)
DECLARE @strSQL_Alter NVARCHAR(MAX)
DECLARE @strSQL_Update NVARCHAR(MAX)
DECLARE @strSQL_Update_AtSeven NVARCHAR(MAX)

DECLARE @measDesc NVARCHAR(MAX)

IF OBJECT_ID('tempdb..#KeyIndicators') IS NOT NULL 
	BEGIN
		DROP TABLE #KeyIndicators
	END

BEGIN
SET @strMonth =
( CASE @intMonth 
	WHEN 1 THEN 'Jan'
	WHEN 2 THEN 'Feb'
	WHEN 3 THEN 'Mar'
	WHEN 4 THEN 'Apr'
	WHEN 5 THEN 'May'
	WHEN 6 THEN 'Jun'
	WHEN 7 THEN 'Jul'
	WHEN 8 THEN 'Aug'
	WHEN 9 THEN 'Sep'
	WHEN 10 THEN 'Oct'
	WHEN 11 THEN 'Nov'
	WHEN 12 THEN 'Dec'
	ELSE
		'Error'
	End	)
END

CREATE TABLE #KeyIndicators
	(
	 License_ID INT
	,Region NVARCHAR(MAX)
	,License NVARCHAR(MAX)
	,Year INT
	,Month INT
	)
	
BEGIN

IF @strRegion LIKE 'ALL'
	BEGIN
INSERT INTO #KeyIndicators (Region,License, License_ID,Year,Month)
	SELECT 
			license.region
			,license.license_name
			,license.license_id
			,@intYear
			,@intMonth
	FROM
			license
	WHERE
			license.Status LIKE 'A'
	AND
			license.license_id NOT IN (107,109)
	END		
ELSE
BEGIN
INSERT INTO #KeyIndicators (Region,License, License_ID)
	SELECT 
			license.region
			,license.license_name
			,license.license_id
	FROM
			license
	WHERE
			(license.Status LIKE 'A')
	AND
			(license.license_id NOT IN (107,109))
	AND
			(license.region LIKE @strRegion)		
END
			
END	
	
DECLARE crsMeasDesc CURSOR FOR
	SELECT DISTINCT RLS.measurement_desc FROM reports_license_summary RLS
	
	OPEN crsMeasDesc
		FETCH NEXT FROM crsMeasDesc INTO @measDesc
			WHILE @@FETCH_STATUS = 0
				BEGIN	
					SET @strSQL_Alter = (CASE @measDesc 
										WHEN 'Monthly Rental Total' THEN
											N'
												ALTER TABLE #KeyIndicators ADD [' + @measDesc + '] DECIMAL (18,2)  
											    ALTER TABLE #KeyIndicators ADD [' + @measDesc +' at 7] DECIMAL (18,2) 
											 '
										ELSE
											N'ALTER TABLE #KeyIndicators ADD [' + @measDesc + '] BIGINT '
									END
								  )
						
						BEGIN			  
						SET @strSQL_Update = ( 
											'UPDATE #KeyIndicators 
												SET ['+@measDesc+'] = (
																		SELECT 
																				SUM('+ @strMonth+')
																		FROM 
																				reports_license_summary RLS 
																		WHERE 
																				(YEAR = '+ @intYear+')  
																		AND 
																				(RLS.license_id = #KeyIndicators.License_ID ) 
																		AND 
																				(RLS.measurement_desc LIKE  '''+@measDesc+''')
																	 )'							 
										 )
						END
							
						
							
							
							
						EXEC (@strSQL_Alter)
						EXEC (@strSQL_Update)
						
						
						--UPDATE #KeyIndicators
						--	SET [Monthly Rental Total at 7] = [Monthly Rental Total]/0.07
								  
						FETCH NEXT FROM crsMeasDesc INTO  @measDesc
				END

BEGIN
							SET @strSQL_Update_AtSeven = 'UPDATE #KeyIndicators
															SET [Monthly Rental Total at 7] = (
																		SELECT 
																				SUM('+ @strMonth+')/0.07
																		FROM 
																				reports_license_summary RLS 
																		WHERE 
																				(YEAR = '+ @intYear+')  
																		AND 
																				(RLS.license_id = #KeyIndicators.License_ID ) 
																		AND 
																				(RLS.measurement_desc LIKE  ''Monthly Rental Total'')
																	 )'	
						END					
				
EXEC (@strSQL_Update_AtSeven)				
				
				
	CLOSE crsMeasDesc
	DEALLOCATE crsMeasDesc

SELECT 
		--Region
		License
		,ISNULL(Agents,0) AS [Agents]
		,ISNUll([Monthly Rental Total],0) AS [Monthly Rental Total]
		,ISNUll([Monthly Rental Total at 7],0) AS [Monthly Rental Total at 7] 
		,ISNUll([New Sole Mandates],0) AS [New Sole Mandates]
		,ISNULL([Show Days],0) AS [Show Days]
		,ISNULL([Sole Mandates],0)	AS [Sole Mandates]		
FROM #KeyIndicators

UNION SELECT
		'ZZZ_Grand Total'
		,ISNULL(SUM(Agents),0) AS [Agents]
		,ISNUll(SUM([Monthly Rental Total]),0) AS [Monthly Rental Total]
		,ISNUll(SUM([Monthly Rental Total at 7]),0) AS [Monthly Rental Total at 7] 
		,ISNUll(SUM([New Sole Mandates]),0) AS [New Sole Mandates]
		,ISNULL(SUM([Show Days]),0) AS [Show Days]
		,ISNULL(SUM([Sole Mandates]),0)	AS [Sole Mandates]		
FROM #KeyIndicators

ORDER BY  License



DROP TABLE #KeyIndicators
END



