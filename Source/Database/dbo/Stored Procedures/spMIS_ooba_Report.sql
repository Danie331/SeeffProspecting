
-- =============================================
-- Author:		Gustav Swanepoel
-- Create date: 2012-03-09
-- Description:	Stored procedure for returning
--				the resultset for the monthly
--				ooba report
-- =============================================
CREATE PROCEDURE [dbo].[spMIS_ooba_Report] 
	-- Add the parameters for the stored procedure here
	@Year INT,
	@Month INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
			 SELECT     license.license_name AS [License]
					, branch.branchName AS [Branch]
					, dbo.credited_agents(sps_tran.sps_transaction_ref) AS [Agent]
                    , sps_tran.sps_transaction_id AS [Transaction ID]	
                    , sps_tran.sps_reporting_date  AS [Reporting Date]
                    , sps_tran.sps_selling_price AS [Selling Price]
                    
                    , dbo.sales_comm_at_seven
								(sps_tran.sps_refferal_type
								, sps_tran.sps_referral_comm
								, sps_tran.sps_comm_amount) AS [Sale at Seven Percent]
                    
                    , dbo.sales_comm_perc
								(sps_tran.sps_refferal_type
								, sps_tran.sps_referral_comm
								, sps_tran.sps_comm_amount
								, sps_tran.sps_selling_price) AS [Commission Percentage]
                    
                    , sps_tran.sps_transaction_division AS [Division]
                    
                     , dbo.sales_actual_selling(sps_tran.sps_refferal_type, sps_tran.sps_referral_comm, sps_tran.sps_comm_amount, 
                      sps_tran.sps_selling_price) AS [Actual Selling Price]                    
                    , sps_tran.sps_property_address	 AS [Address]
                    , dbo.person_by_roll(sps_tran.sps_transaction_ref, 'seller') AS [Seller]
                    , dbo.person_by_roll(sps_tran.sps_transaction_ref, 'buyer') AS [Buyer]
FROM				
					sps_transaction AS sps_tran 
INNER JOIN
                    license_branches ON sps_tran.branch_id = license_branches.branch_id 
INNER JOIN
                    license ON license_branches.license_id = license.license_id 
INNER JOIN
                    branch ON sps_tran.branch_id = branch.branchId 
INNER JOIN
                    user_registration ON sps_tran.created_by = user_registration.registration_id
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
					(YEAR(sps_tran.sps_reporting_date) = @Year)  
AND 
					(MONTH(sps_tran.sps_reporting_date) = @Month)
ORDER BY license.license_name,branch.branchName  ASC
END

