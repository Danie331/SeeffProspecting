



-- =============================================
-- Author:		Gustav Swanepoel
-- Create date: 2012-05-31
-- Description:	License Cancelled Transactions
--				Division Summary Sheet
--				CHANGED CALCULATION OF
--				COMMISSION PERCENTAGE
-- =============================================
CREATE PROCEDURE [dbo].[spMIS_Lic_CanTran_Summary]
	-- Add the parameters for the stored procedure here
	@License_ID AS INTEGER,
	@DateBegin as NVARCHAR(20),
	@DateEnd As NVARCHAR(20)

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

SELECT   
			 YEAR(sps_tran.sps_reporting_date) AS [Year Reported] 
			 ,MONTH(sps_tran.sps_reporting_date) AS [Month Reported]
			 , license.license_name AS [License]
			 , SUM(dbo.sales_units(sps_tran.sps_refferal_type)) AS [Unit Count]
			 , SUM(dbo.sales_actual_selling(sps_tran.sps_refferal_type, 
											sps_tran.sps_referral_comm, 
											sps_tran.sps_comm_amount, 
											sps_tran.sps_selling_price)) AS [Total Actual Selling]
			 , SUM(dbo.sales_comm_at_seven(sps_tran.sps_refferal_type, 
										   sps_tran.sps_referral_comm, 
										   sps_tran.sps_comm_amount)) AS [Sales At Seven Perc]
			 , AVG(dbo.sales_comm_perc(sps_tran.sps_refferal_type, sps_tran.sps_referral_comm, sps_tran.sps_comm_amount
									   , sps_tran.sps_selling_price)) AS [Comm Perc]
			 , SUM(dbo.sales_company_comm(sps_tran.sps_refferal_type, ISNULL(sps_tran.sps_referral_comm, 0),sps_tran.sps_comm_amount)) AS [Comm Amount]												   
FROM         
			sps_transaction AS sps_tran
INNER JOIN
			license_branches ON sps_tran.branch_id = license_branches.branch_id 
INNER JOIN
			license ON license_branches.license_id = license.license_id
WHERE     
			(sps_tran.sps_cancelled = 1) 
--AND 
--			(sps_tran.sps_transaction_type = 'Sale') 
AND 
			(ISNULL(sps_tran.sps_comm_amount, 0) > 0) 
AND 
			(ISNULL(sps_tran.sps_selling_price, 0) > 0) 
AND 
			(sps_tran.branch_id NOT IN (260, 263, 277)) 
AND 
			(sps_tran.sps_reporting_date >= @DateBegin)
AND
			(sps_tran.sps_reporting_date <= @DateEnd)
AND
			(license.license_id = @License_ID)			
			
GROUP BY 
			 license.license_name
			, license_branches.license_id
			,YEAR(sps_tran.sps_reporting_date)
			,MONTH(sps_tran.sps_reporting_date)
										  	
ORDER BY  
			YEAR(sps_tran.sps_reporting_date)
			,MONTH(sps_tran.sps_reporting_date)
			 DESC
END




