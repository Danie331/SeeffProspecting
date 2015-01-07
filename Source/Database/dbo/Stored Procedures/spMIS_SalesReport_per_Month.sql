CREATE PROCEDURE [dbo].[spMIS_SalesReport_per_Month] 
	-- Add the parameters for the stored procedure here
	  @Month INT = 1
	, @Year INT = 2013
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
SELECT     
		   license.license_id AS [License ID]
		   , branch.branchId AS [Branch ID]
		   ,dbo.fnMIS_SubRegion(license.license_id)AS [Sales Sub Region]
		   , sps_transaction.sps_transaction_division AS [Division]
		   , license.license_name AS [License]
		   , branch.branchName AS [Branch] 
		   , sps_transaction.sps_lic_deal_no AS [License Deal No]
		   , sps_transaction.sps_refferal_type AS [Referral Type]
		   , ISNULL(sps_transaction.sps_referral_comm, 0) AS [Referral Comm]
		   , dbo.sales_actual_selling(sps_transaction.sps_refferal_type, sps_transaction.sps_referral_comm, sps_transaction.sps_comm_amount, 
		         sps_transaction.sps_selling_price) AS [Actual Selling Price]
		   , dbo.sales_comm_at_seven(sps_transaction.sps_refferal_type, sps_transaction.sps_referral_comm, sps_transaction.sps_comm_amount) AS [Sale at Seven Percent]
		    , sps_transaction.sps_comm_amount AS [Comm Amount]
           , dbo.sales_comm_perc(sps_transaction.sps_refferal_type, sps_transaction.sps_referral_comm, sps_transaction.sps_comm_amount
           , sps_transaction.sps_selling_price) AS [Comm Percentage]
		   , sps_transaction.sps_listing_price AS [Listing Price]
		   , sps_transaction.sps_selling_price AS [Selling Price]
           , sps_transaction.sps_listing_price - sps_transaction.sps_selling_price AS [Sale Less List] 
           , (sps_transaction.sps_listing_price - sps_transaction.sps_selling_price) / NULLIF (sps_transaction.sps_listing_price, 0) AS [Percentage Diff]
           , sps_transaction.sps_reporting_date AS [Date Reported]
		   , sps_transaction.sps_listed_date AS [Date Listed]
		   , sps_transaction.sps_sold_date AS [Date Sold]
		   , DATEDIFF(WEEK, sps_transaction.sps_listed_date, sps_transaction.sps_sold_date) AS [Weeks Difference]
           , CASE [sps_buyer_source] WHEN 'Please Select' THEN 'Not Selected' ELSE [sps_buyer_source] END AS [Buyer Source] 
           , CASE [sps_seller_source] WHEN 'Please Select' THEN 'Not Selected' ELSE [sps_seller_source] END AS [Seller Source]
           , ISNULL(sps_transaction.sps_showdays, 
                      0) AS [Showdays]
           , sps_transaction.sps_solemandate AS [Sole Mandate]
           , sps_transaction.sps_transferring_attorney AS [Trans Attorney]
           , sps_transaction.sps_bond_attorney AS [Bond Attorney]
           , sps_transaction.sps_property_id AS [Property ID]
           , sps_transaction.sps_property_address AS [Property Address]
           , user_registration.user_preferred_name + ' ' + user_registration.user_surname AS [Captured By]
           , ISNULL(dbo.credited_agents(sps_transaction.sps_transaction_ref),'') AS [Agent]
           
INTO #SalesReport_Per_Month           
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
					(YEAR(sps_transaction.sps_reporting_date) = @Year) 
AND 
					(MONTH(sps_transaction.sps_reporting_date) = @Month)
ORDER BY 
					sps_transaction.sps_transaction_type, 
					sps_transaction.sps_reporting_date
					
UPDATE #SalesReport_Per_Month
	SET [License] = 'Karoo'
						WHERE #SalesReport_Per_Month.[Branch ID] IN (72,156,172,174,74,75,76,220,210,208,233,322,82,330,324,332,333,334,340)						
UPDATE #SalesReport_Per_Month
	SET [License] = 'Southern Cape'
						WHERE #SalesReport_Per_Month.[Branch ID] IN (94,102,104,106,108,119,127,139,157,159,173,175,183,185,197,201,224,227,242,314,319,331,325)						
							
					
SELECT * FROM #SalesReport_Per_Month ORDER BY [Sales Sub Region],[License] ASC

DROP TABLE #SalesReport_Per_Month

END