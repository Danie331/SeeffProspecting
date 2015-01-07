

-- =============================================
-- Author:		Gustav Swanepoel
-- Create date: 2011-10-06
-- Description:	From Adam Roberts query
--				Pass month and year variables (INT)
--				to return resultset used for
--				drawing monthly Sales Report
--				(in its existing form)
-- =============================================
CREATE PROCEDURE [dbo].[spMIS_SalesDetail] 
	-- Add the parameters for the stored procedure here
	@License_ID INT
	, @Begin VARCHAR(20)
	, @End VARCHAR(20)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
SELECT     
			RIGHT(CONVERT(VARCHAR(10), sps_transaction.sps_reporting_date, 105), 7) AS [Year_Month_Reported]
		   , sps_transaction.sps_transaction_division
		   , branch.branchName
		   , ISNULL(dbo.credited_agents(sps_transaction.sps_transaction_ref),'') AS Agent
		   , sps_transaction.sps_lic_deal_no
		   , sps_transaction.sps_reporting_date
		   , sps_transaction.sps_refferal_type
		   , ISNULL(sps_transaction.sps_referral_comm, 0) AS sps_referral_comm
		   , sps_transaction.sps_listed_date
		   , sps_transaction.sps_sold_date
		   , DATEDIFF(WEEK, sps_transaction.sps_listed_date, sps_transaction.sps_sold_date) AS weeks_diff
		   , sps_transaction.sps_listing_price
		   , sps_transaction.sps_selling_price
		   
		   , dbo.sales_actual_selling(sps_transaction.sps_refferal_type, sps_transaction.sps_referral_comm, sps_transaction.sps_comm_amount, 
		         sps_transaction.sps_selling_price) AS actual_selling
           
           , sps_transaction.sps_listing_price - sps_transaction.sps_selling_price AS sale_less_list 
           
           , (sps_transaction.sps_listing_price - sps_transaction.sps_selling_price) / NULLIF (sps_transaction.sps_listing_price, 0) AS perc_diff 
           
           , dbo.sales_comm_at_seven(sps_transaction.sps_refferal_type, sps_transaction.sps_referral_comm, sps_transaction.sps_comm_amount) AS sale_at_seven
           
           , dbo.sales_comm_perc(sps_transaction.sps_refferal_type, sps_transaction.sps_referral_comm, sps_transaction.sps_comm_amount
			, sps_transaction.sps_selling_price) AS comm_perc
           
           , sps_transaction.sps_comm_amount
                           
           , CASE [sps_buyer_source] WHEN 'Please Select' THEN 'Not Selected' ELSE [sps_buyer_source] END AS sps_buyer_source 
           , CASE [sps_seller_source] WHEN 'Please Select' THEN 'Not Selected' ELSE [sps_seller_source] END AS sps_seller_source
           , ISNULL(sps_transaction.sps_showdays, 
                      0) AS sps_showdays
           , sps_transaction.sps_solemandate
           , sps_transaction.sps_transferring_attorney
           , sps_transaction.sps_bond_attorney
           , sps_transaction.sps_property_id
           , sps_transaction.sps_property_address
           , ISNULL(dbo.person_by_roll(sps_transaction.sps_transaction_ref, 'seller'),'') AS seller
           , ISNULL(dbo.person_by_roll(sps_transaction.sps_transaction_ref, 'buyer'),'') AS buyer
           , user_registration.user_preferred_name + ' ' + user_registration.user_surname AS captured_by
           , license.license_name
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
			(license.license_id = @License_ID)					
AND 
			(sps_transaction.sps_reporting_date >= @Begin) 
AND 
			(sps_transaction.sps_reporting_date <= @End)
ORDER BY 
			sps_transaction.sps_transaction_type, 
			sps_transaction.sps_reporting_date
END





