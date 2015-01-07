-- =============================================
-- Author:		GW Swanepoel
-- Create date: 2012-06-07
-- Modifi date:	2013-11-22 - 24
-- Description:	From table reports_license_summary
--				Returns only Monthly Rental Total
--				measurement description
--2013-11-24:	now dynamic for financial year
--				based on the day on which
--				this sp gets executed
-- =============================================
CREATE PROCEDURE [dbo].[spMIS_Lic_Monthly_Rental_Total]
	-- Add the parameters for the stored procedure here
	@YearFor NVARCHAR(4) ='2013'
	,@License_ID NVARCHAR(10) = 113
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here

	DECLARE @CurrMonth INT 
		SET @CurrMonth = (SELECT DATEPART(M,GETDATE()))
	
	DECLARE @CurrYear INT
		SET @CurrYear = (SELECT DATEPART(YYYY,GETDATE()))

		--financial year is begin Nov to End Oct
		--	if month is 11 and year is 2013 then
		--	in fin year 2013-14
		--	then report on	N13 D13 J14 F14 M14 A14  
		--					M14 J14 J14 A14 S14 O14

		--from Jan - Oct:
		DECLARE @Year1 INT
		DECLARE @Year2 INT

		
		SELECT @Year1 = CASE 
						WHEN @CurrMonth  >=11 AND @CurrMonth  <=12 THEN (@CurrYear)
						WHEN @CurrMonth  >=1 AND @CurrMonth <=10 THEN (@CurrYear-1)
						ELSE
						@CurrYear
						END

		SELECT @Year2 = CASE 
						WHEN @CurrMonth >= 11 AND @CurrMonth <= 12 THEN (@CurrYear+1)
						WHEN @CurrMonth >= 1 AND @CurrMonth <= 10 THEN (@CurrYear)
						ELSE
						@CurrYear
						END

--GW:	test on something that has values
--SET @Year1 = 2012 --//GW: this works
--SET @Year2 = 2013 --//GW: this works


IF OBJECT_ID(N'MonthRentTotal', N'U') IS NOT NULL DROP TABLE MonthRentTotal

DECLARE @strSQL VARCHAR(1000)
SET @strSQL = 'CREATE TABLE MonthRentTotal 
				([seq] int, [Measurement Description] VARCHAR(100)
				,Year VARCHAR(100), Total DECIMAL(18,2),
				[Nov ' + CAST(@Year1 as varchar(4)) + '] DECIMAL(18,2),  [Dec ' + CAST(@Year1 as varchar(4)) + '] DECIMAL(18,2), 
				[Jan ' + CAST(@Year2 as varchar(4)) + '] DECIMAL(18,2),  [Feb ' + CAST(@Year2 as varchar(4)) + '] DECIMAL(18,2),
				[Mar ' + CAST(@Year2 as varchar(4)) + '] DECIMAL(18,2),  [Apr ' + CAST(@Year2 as varchar(4)) + '] DECIMAL(18,2),
				[May ' + CAST(@Year2 as varchar(4)) + '] DECIMAL(18,2),  [Jun ' + CAST(@Year2 as varchar(4)) + '] DECIMAL(18,2),
				[Jul ' + CAST(@Year2 as varchar(4)) + '] DECIMAL(18,2),  [Aug ' + CAST(@Year2 as varchar(4)) + '] DECIMAL(18,2),
				[Sep ' + CAST(@Year2 as varchar(4)) + '] DECIMAL(18,2),  [Oct ' + CAST(@Year2 as varchar(4)) + '] DECIMAL(18,2));'


print @strSQL
EXECUTE (@strSQL)

DECLARE @CountYear2 INT 
		,@CountYear1 INT
SET @CountYear2 = (SELECT COUNT(*) FROM reports_license_summary WHERE 
		(year = @Year2)
AND 
		(license_id = @License_ID)
AND 
		(measurement_desc LIKE 'Monthly Rental Total') 
		)

SET @CountYear1 = (SELECT COUNT(*) FROM reports_license_summary WHERE 
		(year = @Year1)
AND 
		(license_id = @License_ID)
AND 
		(measurement_desc LIKE 'Monthly Rental Total') 
		)

	INSERT MonthRentTotal 
	SELECT 1
	, 'Monthly Rental Total'
	,CAST(@Year1 AS VARCHAR(4)) + '/' + CAST(@Year2 AS VARCHAR(4))
	, 0
	,0 AS Nov
	,0 AS Dec
	, 0
	, 0
	, 0
	, 0
	, 0
	, 0
	, 0
	, 0
	, 0
	, 0
	

INSERT MonthRentTotal 
	SELECT  2, measurement_desc
			,CAST(@Year1 AS VARCHAR(4)) + '/' + CAST(@Year2 AS VARCHAR(4))
			,0
			,0 AS Nov
			,0 AS Dec
			, ISNULL(SUM(jan),0) AS [Jan]
			, ISNULL(SUM(feb),0) AS [Feb]
			, ISNULL(SUM(mar),0) AS [Mar]
			, ISNULL(SUM(apr),0) AS [Apr]
			, ISNULL(SUM(may),0) AS [May]
			, ISNULL(SUM(jun),0) AS [Jun]
			, ISNULL(SUM(jul),0) AS [Jul]
			, ISNULL(SUM(aug),0) AS [Aug]
			, ISNULL(SUM(sep),0) AS [Sep]
			, ISNULL(SUM(oct),0) AS [Oct]
FROM			 
				reports_license_summary
WHERE 
		(year = @Year2)
AND 
		(license_id = @License_ID)
AND 
		(measurement_desc LIKE 'Monthly Rental Total')
GROUP BY 
		measurement_desc
		, year
		, license_id


IF @CountYear1 <> 0 
BEGIN
SET @strSQL = 'UPDATE MonthRentTotal 
				SET [Nov ' + CAST(@Year1 as varchar(4)) + '] = 
					(SELECT 
							ISNULL(SUM(Nov),0) 
					 FROM 
							reports_license_summary 
					 WHERE					 
							(year = ' + CAST(@Year1 as varchar(4)) + ')
					 AND 
							(license_id = ' + CAST(@License_ID as varchar(4)) + ')
					 AND 
							(measurement_desc LIKE ''Monthly Rental Total'')
					 GROUP BY 
							measurement_desc
							, year
							, license_id 
							)'
PRINT (@strSQL)
EXECUTE (@strSQL)

SET @strSQL = 'UPDATE MonthRentTotal 
				SET [Dec ' + CAST(@Year1 as varchar(4)) + '] = 
					(SELECT 
							ISNULL(SUM(Dec),0) 
					 FROM 
							reports_license_summary 
					 WHERE					 
							(year = ' + CAST(@Year1 as varchar(4)) + ')
					 AND 
							(license_id = ' + CAST(@License_ID as varchar(4)) + ')
					 AND 
							(measurement_desc LIKE ''Monthly Rental Total'')
					 GROUP BY 
							measurement_desc
							, year
							, license_id 
							)'
print @strSQL
EXECUTE (@strSQL)
END
--GW:		remember the total buddy
--AR:       added the seq 
SET @strSQL = 'UPDATE MonthRentTotal 
				SET [Total] = 
					(SELECT 
							ISNULL([Nov ' + CAST(@Year1 as varchar(4)) + '],0) +  ISNULL([Dec ' + CAST(@Year1 as varchar(4)) + '],0) + 
							ISNULL([Jan ' + CAST(@Year2 as varchar(4)) + '],0) +  ISNULL([Feb ' + CAST(@Year2 as varchar(4)) + '],0) +
							ISNULL([Mar ' + CAST(@Year2 as varchar(4)) + '],0) +  ISNULL([Apr ' + CAST(@Year2 as varchar(4)) + '],0)+
							ISNULL([May ' + CAST(@Year2 as varchar(4)) + '],0) +  ISNULL([Jun ' + CAST(@Year2 as varchar(4)) + '],0) +
							ISNULL([Jul ' + CAST(@Year2 as varchar(4)) + '],0) +  ISNULL([Aug ' + CAST(@Year2 as varchar(4)) + '],0) +
							ISNULL([Sep ' + CAST(@Year2 as varchar(4)) + '],0) +  ISNULL([Oct ' + CAST(@Year2 as varchar(4)) + '],0)  
					FROM
							MonthRentTotal
                    WHERE seq = 2 
					)'
PRINT @strSQL
EXECUTE (@strSQL)		



--GW:	finally: we return the results 
--		for the particular financial year	

		SELECT [Measurement Description]
      ,[Year]
      ,[Total]
      ,[Nov 2014]
      ,[Dec 2014]
      ,[Jan 2015]
      ,[Feb 2015]
      ,[Mar 2015]
      ,[Apr 2015]
      ,[May 2015]
      ,[Jun 2015]
      ,[Jul 2015]
      ,[Aug 2015]
      ,[Sep 2015]
      ,[Oct 2015]
  FROM [boss].[dbo].[MonthRentTotal]
WHERE [seq] = 2

END

