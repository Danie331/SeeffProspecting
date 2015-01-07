
-- =============================================
-- Author:		Gustav Swanepoel
-- Create date: 2012-10-08
-- Description:	License Cancelled Transactions
--				Division Summary Sheet
--				CHANGED CALCULATION OF
--				COMMISSION PERCENTAGE
-- =============================================
CREATE PROCEDURE [dbo].[spMIS_Rgn_CanTran_Summary]
	-- Add the parameters for the stored procedure here
	--@Region AS NVARCHAR(20)= 'International',
	@DateBegin as NVARCHAR(20) = '2012-01-01',
	@DateEnd As NVARCHAR(20) = '2012-09-30'

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

SELECT   
			 YEAR(sps_tran.sps_reporting_date) AS [Year Reported] 
			 
			 ,MONTH(sps_tran.sps_reporting_date) AS [Month Reported]
			 
			 , license.region AS [Region]
			 
			 --, license.sub_region AS [Sub Region]
			 
			 , dbo.fnMIS_SubRegion(license.license_id)  AS [Sub Region]
			 
			 , license.license_name AS [License Name]
			 
			 , sps_tran.sps_transaction_division AS [Division]
			 
			 , COUNT(sps_tran.sps_transaction_division) AS [Unit Count(Cancelled)]
			 
			 , SUM(dbo.sales_actual_selling(sps_tran.sps_refferal_type, 
											sps_tran.sps_referral_comm, 
											sps_tran.sps_comm_amount, 
											sps_tran.sps_selling_price)) AS [Total Actual Selling(Cancelled)]
			 
			 , SUM(dbo.sales_comm_at_seven(sps_tran.sps_refferal_type, 
										   sps_tran.sps_referral_comm, 
										   sps_tran.sps_comm_amount)) AS [Sales At Seven Perc (Cancelled)]
			 
			 , (SUM(dbo.fnMIS_sales_commission(sps_tran.sps_refferal_type, ISNULL(sps_tran.	sps_referral_comm, 0),sps_tran.sps_comm_amount))) 
					/ 
			   (SUM (sps_tran.sps_selling_price)) AS [Comm Perc]
			 
			 , SUM(dbo.sales_company_comm(sps_tran.sps_refferal_type, ISNULL(sps_tran.sps_referral_comm, 0),sps_tran.sps_comm_amount)) AS [Comm Amount (Cancelled)]												   
FROM         
			sps_transaction AS sps_tran
INNER JOIN
			license_branches ON sps_tran.branch_id = license_branches.branch_id 
INNER JOIN
			license ON license_branches.license_id = license.license_id
WHERE     
			(sps_tran.sps_cancelled = 1) 
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
			(sps_tran.sps_transaction_division LIKE 'Residential')						
GROUP BY 
			 license.region
			, license.license_id
			,YEAR(sps_tran.sps_reporting_date)
			,MONTH(sps_tran.sps_reporting_date)	
			,sps_tran.sps_transaction_division	
			,license.sub_region
			,license.license_name								  	
ORDER BY  
			YEAR(sps_tran.sps_reporting_date)
			,MONTH(sps_tran.sps_reporting_date)
			 DESC
	--IF @@ROWCOUNT = 0
	--	BEGIN
			
	--		SELECT   
	--		 YEAR(@DateBegin) AS [Year Reported] 
	--		 ,MONTH(@DateBegin) AS [Month Reported]
	--		 , @Region AS [Region]
	--		 , 0 AS [Unit Count]
	--		 , 0 AS [Total Actual Selling]
	--		 , 0 AS [Sales At Seven Perc]
	--		 , 0 AS [Comm Perc]
	--		 , 0 AS [Comm Amount]												   
					 
	--	END	 
			 
END




