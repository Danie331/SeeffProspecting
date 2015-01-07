
-- =============================================
-- Author:		Gustav Swanepoel
-- Create date: 2012-04-23
-- Description:	Hub Field Analysis Report
--				Returns transactions where
--				Referral Type is not 
--				'External Paid to You '
-- =============================================
CREATE PROCEDURE [dbo].[spMIS_HFA_03_NotExtPaidYToYou]
	-- Add the parameters for the stored procedure here
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	CREATE NONCLUSTERED INDEX Indx01 ON [dbo].[sps_agent_split]([sps_transaction_ref]) INCLUDE ([registration_id],[comm_paid],[recognise])

SELECT     
			user_registration_1.user_preferred_name + ' ' + user_registration_1.user_surname AS [Created By]
			, sps_transaction.sps_lic_deal_no AS [Lic Deal No]
			, sps_transaction.sps_transaction_id AS [Trans ID]
			, sps_hub_transaction.transaction_status AS [Tran Status]
			, dbo.credited_agents(sps_transaction.sps_transaction_ref) AS [Agent]
			, sps_transaction.sps_refferal_type AS [Ref Type]
			, sps_transaction.sps_reporting_date AS [Reporting Date]
			, sps_transaction.sps_sold_date AS [Sold Date]
			, sps_transaction.sps_selling_price AS [Sell Price]
			, sps_transaction.sps_referral_comm AS [Ref Comm]
			, sps_transaction.sps_comm_amount AS [Comm Amount]
			, sps_transaction.sps_percentage_split AS [Perc Split]
			, sps_transaction.sps_listing_price AS [Listing Price]
			, ISNULL(dbo.person_by_roll(sps_transaction.sps_transaction_ref, 'seller'), '') AS [Seller]
			, ISNULL(dbo.person_by_roll(sps_transaction.sps_transaction_ref, 'buyer'), '') AS [Buyer]
			,sps_transaction.sps_comment AS [Comment]
			,sps_transaction.sps_expected_reg_date AS [Expected Reg Date]
FROM         
			sps_transaction 
INNER JOIN
            sps_hub_transaction ON	sps_transaction.sps_transaction_ref = sps_hub_transaction.sps_transaction_ref 
INNER JOIN
            license_branches ON sps_transaction.branch_id = license_branches.branch_id 
INNER JOIN
            license ON license_branches.license_id = license.license_id 
INNER JOIN
            user_registration ON sps_hub_transaction.registration_id = user_registration.registration_id 
INNER JOIN
			user_registration AS user_registration_1 ON sps_transaction.created_by = user_registration_1.registration_id				
WHERE     
			(sps_transaction.sps_refferal_type <> 'External paid to you')
																								
GROUP BY
			  sps_transaction.sps_reporting_date
			, license.license_name
			, sps_transaction.sps_lic_deal_no
			, dbo.credited_agents(sps_transaction.sps_transaction_ref) 
			, sps_transaction.sps_refferal_type
			, sps_transaction.sps_referral_comm
			, sps_transaction.sps_sold_date
			, sps_transaction.sps_selling_price
			, sps_transaction.sps_transaction_id
			, sps_hub_transaction.transaction_status
			, sps_transaction.sps_reporting_date
			, sps_transaction.sps_percentage_split
			, sps_transaction.sps_listing_price
			, sps_transaction.sps_transaction_type
			, sps_transaction.branch_id
			, sps_transaction.sps_comm_amount
			, sps_transaction.sps_comment
			, sps_transaction.sps_expected_reg_date
			, user_registration.user_preferred_name
			, user_registration.user_surname
			, user_registration_1.user_preferred_name
			, user_registration_1.user_surname
			, sps_transaction.sps_transaction_ref

HAVING
			(sps_transaction.sps_reporting_date IS NOT NULL)
							
ORDER BY 
			user_registration_1.user_preferred_name + ' ' + user_registration_1.user_surname 
			
DROP INDEX Indx01 ON [dbo].[sps_agent_split]
END

