
-- =============================================
-- Author:		Gustav Swanepoel
-- Create date: 2011-10-18
-- Description:	From Adam Roberts query
--				Pass month and year variables (INT)
--				to return resultset used for
--				drawing monthly Sales Report
--				(in its existing form)
--				See changes and comments on lines
--				65 - 68
-- =============================================
CREATE PROCEDURE [dbo].[spMIS_License_Sales_Per_Fin_Year_2010_2011] 
	-- Add the parameters for the stored procedure here
	  @License_ID INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
SELECT     
			 sbf_seeff_billed AS [SBF Batch Date]
		   , sbf_seeff_fee AS [SBF Fee]	
		   , sps_transaction.sps_transaction_division AS [Transaction Division]
		   , license.license_name AS [License]
		   , branch.branchName AS [Branch]
		   , ISNULL(dbo.credited_agents(sps_transaction.sps_transaction_ref),'') AS [Agent]
		   , sps_transaction.sps_lic_deal_no AS [License Deal No]
		   , RIGHT(CONVERT(VARCHAR(10), sps_transaction.sps_reporting_date, 105), 7) AS [Month Year Reported]
		   , sps_transaction.sps_reporting_date  AS [Reporting Date]
		   , sps_transaction.sps_refferal_type AS [Referral Type]
		   , ISNULL(sps_transaction.sps_referral_comm, 0) AS [Referral Comm]
		   , sps_transaction.sps_listed_date AS [Listed Date]
		   , sps_transaction.sps_sold_date AS [Sold Date]
		   , DATEDIFF(WEEK, sps_transaction.sps_listed_date, sps_transaction.sps_sold_date) AS [Weeks Difference]
		   , sps_transaction.sps_listing_price AS [Listing Price]
		   , sps_transaction.sps_selling_price AS [Selling Price]
		   
		   , dbo.sales_actual_selling(sps_transaction.sps_refferal_type, sps_transaction.sps_referral_comm, sps_transaction.sps_comm_amount, 
		         sps_transaction.sps_selling_price) AS [Actual Selling]
           
           , sps_transaction.sps_listing_price - sps_transaction.sps_selling_price AS [Sale Less List]
           
           , (sps_transaction.sps_listing_price - sps_transaction.sps_selling_price) / NULLIF (sps_transaction.sps_listing_price, 0) AS [Percentage Difference] 
           
           , dbo.sales_comm_at_seven(sps_transaction.sps_refferal_type, sps_transaction.sps_referral_comm, sps_transaction.sps_comm_amount) AS [Sale At Seven Perc]
           
           /*, sps_transaction.sps_comm_amount AS [Comm Amount]*/
           /*Changed on feedback from Mark Jaftha to this for 
             calculation of commission: */
           , dbo.sales_company_comm(sps_transaction.sps_refferal_type, ISNULL(sps_transaction.sps_referral_comm, 0),sps_transaction.sps_comm_amount) AS [Comm Amount]
           
           , dbo.sales_comm_perc(sps_transaction.sps_refferal_type, sps_transaction.sps_referral_comm, sps_transaction.sps_comm_amount
           , sps_transaction.sps_selling_price) AS [Comm Perc]
           
           , CASE [sps_buyer_source] WHEN 'Please Select' THEN 'Not Selected' ELSE [sps_buyer_source] END AS [Buyer Source] 
           , CASE [sps_seller_source] WHEN 'Please Select' THEN 'Not Selected' ELSE [sps_seller_source] END AS [Seller Source]
           , ISNULL(sps_transaction.sps_showdays, 
                      0) AS [Showdays] 
           , sps_transaction.sps_solemandate AS [Sole mandate]
           , sps_transaction.sps_transferring_attorney AS [Trans Attorney]
           , sps_transaction.sps_bond_attorney AS [Bond Attorney]
           , sps_transaction.sps_property_id AS [Property ID]
           , sps_transaction.sps_property_address AS [Property Address]
           , ISNULL(dbo.person_by_roll(sps_transaction.sps_transaction_ref, 'seller'),'') AS [Seller]
           , ISNULL(dbo.person_by_roll(sps_transaction.sps_transaction_ref, 'buyer'),'') AS [Buyer]
           , user_registration.user_preferred_name + ' ' + user_registration.user_surname AS [Captured By]
		   , dbo.sales_units(sps_transaction.sps_refferal_type) AS [Unit Count]
           
FROM         
				   sps_transaction 
INNER JOIN
           license_branches ON sps_transaction.branch_id = license_branches.branch_id 
INNER JOIN
           license ON license_branches.license_id = license.license_id 
INNER JOIN
           branch ON sps_transaction.branch_id = branch.branchId 
INNER JOIN
           user_registration ON sps_transaction.created_by = user_registration.registration_id
WHERE     
					(sps_transaction.sps_cancelled = 0) 
AND 
					(sps_transaction.sps_transaction_type = 'Sale') 
AND 
					(ISNULL(sps_transaction.sps_comm_amount, 0) > 0) 
AND 
          (ISNULL(sps_transaction.sps_selling_price, 0) > 0) 
AND 
					(sps_transaction.branch_id NOT IN (260, 263, 277)) 
AND 
					sps_transaction.sps_reporting_date
BETWEEN
					'2010/11/01'
AND
					'2011/10/31'					
AND
					license.license_id = @License_ID					
ORDER BY 
					sps_transaction.sps_transaction_type, 
					sps_transaction.sps_reporting_date
END


