


-- =============================================
-- Author:		Gustav Swanepoel	
-- Create date: 2012-03-16
-- Description:	Returns Mandates allocated
--				per license per month
-- =============================================
CREATE PROCEDURE [dbo].[spMIS_Hub_Mandates_Region_ListPrice]
	-- Add the parameters for the stored procedure here
	@DateBegin NVARCHAR(20)
	,@DateEnd NVARCHAR(20)
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	DECLARE @YearMonth NVARCHAR(MAX)

SET @YearMonth = 
			STUFF(
			(
			 SELECT ',[' +  
					 CONVERT(VARCHAR(7), ST.sps_created_date, 120) + ']'
			FROM         
				sps_hub_transaction SHT 
			INNER JOIN
				sps_hub_bank_product SHP ON SHT.product_id = SHP.product_id 
			INNER JOIN
                sps_transaction ST ON SHT.sps_transaction_ref = ST.sps_transaction_ref
			WHERE	
				(ST.sps_created_date >= @DateBegin )
			AND 
				(ST.sps_created_date < = @DateEnd)
			GROUP BY		
				CONVERT(VARCHAR(7), ST.sps_created_date, 120)	
			FOR xml path('')			
			),1,1,''
				) 		

DECLARE @SQL NVARCHAR(MAX)
	
SET @SQL = N'
			SELECT * 
			FROM 
				(
				 SELECT
						ST.sps_listing_price
						,license.region AS [Region]
						,CONVERT(VARCHAR(7), ST.sps_created_date, 120) AS YearMonth
				 FROM         
						sps_hub_transaction AS SHT 						
				 INNER JOIN
                        sps_hub_bank_product AS SHP ON SHT.product_id = SHP.product_id 
                 INNER JOIN
                      sps_transaction AS ST ON SHT.sps_transaction_ref = ST.sps_transaction_ref 
                 INNER JOIN
                      user_registration ON SHT.registration_id = user_registration.registration_id 
                 INNER JOIN
                      license_branches ON user_registration.branch_id = license_branches.branch_id 
                 INNER JOIN
                      license ON license_branches.license_id = license.license_id
                 WHERE 
						(ST.sps_created_date >= ''' + @DateBegin + ''')
                 AND
						(ST.sps_created_date <= ''' + @DateEnd + ''')
				) Data
				PIVOT (SUM(sps_listing_price) FOR YearMonth
				IN	( ' + @YearMonth + ' )
				) PivotTable
				'

EXEC sp_executesql @SQL	
		
END


