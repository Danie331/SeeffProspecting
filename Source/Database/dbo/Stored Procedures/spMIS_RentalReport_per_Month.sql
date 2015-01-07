



-- =============================================
-- Author:		Gustav Swanepoel
-- Create date: 2011-10-06
-- Description:	From Adama Roberts query
--							Pass month and year variables (INT)
--							to return result set used for
--							drawing monthly Rental Report
--							(in its existing form)							
-- =============================================
CREATE PROCEDURE [dbo].[spMIS_RentalReport_per_Month] 
	-- Add the parameters for the stored procedure here
	
	@Month INT
	, @Year INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT     
				  sps_transaction.sps_transaction_division AS [Division]
				, branch.branchName AS [Branch]
				, dbo.credited_agents(sps_transaction.sps_transaction_ref) AS Agent
				, CASE sps_transaction.sps_rental_contract_renewal WHEN 0 THEN 'No' WHEN 1 THEN 'Yes' END AS [RENEWED]
				, '- ' + sps_transaction.sps_lic_deal_no + ' -' AS [Deal No.]
				, sps_transaction.sps_listed_date AS [Start Date]
				, sps_transaction.sps_sold_date AS [End Date]
				, dbo.rental_remaining_period(sps_transaction.sps_sold_date) AS [Expires]
				, sps_transaction.sps_refferal_type AS [Referral Type]
				, ISNULL(sps_transaction.sps_referral_comm, 0) AS [Referral]
				, ISNULL(sps_transaction.sps_comm_amount,0) AS [Non Managed Fee]			
				, ISNULL(sps_transaction.sps_rental_management_fee,0)	AS [Managed Fee]
				, sps_transaction.sps_selling_price AS [Admin Fee]
				, ISNULL(sps_transaction.sps_listing_price,0) AS [Monthly Rental]			
				, sps_transaction.sps_property_address
				, dbo.person_by_roll(sps_transaction.sps_transaction_ref, 'lessor') AS lessor
				, dbo.person_by_roll(sps_transaction.sps_transaction_ref, 'lessee') AS lessee
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
				(sps_transaction.sps_transaction_type = 'Rental') 
AND 
				(YEAR(sps_transaction.sps_listed_date) = @Year) 
AND 
        (MONTH(sps_transaction.sps_listed_date) = @Month)
        
        
        
ORDER BY 
				sps_transaction.sps_transaction_type, 
				sps_transaction.sps_reporting_date
END


