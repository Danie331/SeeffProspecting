
-- =============================================
-- Author:		Gustav Swanepoel
-- Create date: 2012-02-13
-- Description:	National Sales Report per License 
-- =============================================
CREATE PROCEDURE [dbo].[spMIS_Ntnl_Daily_Sales_YTD_per_License]
	-- Add the parameters for the stored procedure here
	
	@DateBegin as NVARCHAR(20),
	@DateEnd as NVARCHAR(20)

AS
BEGIN
	
	SET NOCOUNT ON;

SELECT   
			  '' AS [Division]
			 ,'' AS [Year Month Reported On]
			 ,license.region AS [Region]
			 , license.license_name AS [License]
			 , SUM(dbo.sales_units(sps_tran.sps_refferal_type)) AS [Unit Count]
			 , SUM(dbo.sales_actual_selling(sps_tran.sps_refferal_type, 
											sps_tran.sps_referral_comm, 
											sps_tran.sps_comm_amount, 
											sps_tran.sps_selling_price)) AS [Total Actual Selling]
			 , SUM(dbo.sales_comm_at_seven(sps_tran.sps_refferal_type, 
										   sps_tran.sps_referral_comm, 
										   sps_tran.sps_comm_amount)) AS [Sales At Seven Perc]
			 ,(SUM(dbo.fnMIS_sales_commission(sps_tran.sps_refferal_type, ISNULL(sps_tran.	sps_referral_comm, 0),sps_tran.sps_comm_amount))) 
					/ 
			   (SUM (sps_tran.sps_selling_price)) AS [Comm_Perc]
			,SUM(dbo.sales_company_comm(sps_tran.sps_refferal_type, ISNULL(sps_tran.sps_referral_comm, 0),sps_tran.sps_comm_amount)) AS [Comm Amount]												   
FROM         
			sps_transaction AS sps_tran
INNER JOIN
			license_branches ON sps_tran.branch_id = license_branches.branch_id 
INNER JOIN
			license ON license_branches.license_id = license.license_id
WHERE     
			(sps_tran.sps_cancelled = 0) 
AND 
			(sps_tran.sps_transaction_type = 'Sale') 
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
GROUP BY 
			 license.license_name
			, license_branches.license_id
			, license.region
										  	
ORDER BY 
			License 
			 DESC
END


