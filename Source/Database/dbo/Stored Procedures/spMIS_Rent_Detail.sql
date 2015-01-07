
-- =============================================
-- Author:		Gustav Swanepoel
-- Create date: 2012-03-23
-- Description:	Returns DETAIL transactions listing
--				per license for
--				RENTAL amounts loaded on database
-- =============================================
CREATE PROCEDURE [dbo].[spMIS_Rent_Detail] 
	-- Add the parameters for the stored procedure here
	 @LicenseID INT
	,@Begin NVARCHAR(20)
	,@End NVARCHAR(20)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	CREATE NONCLUSTERED INDEX Idx1 ON [dbo].[sps_agent_split] ([sps_transaction_ref]) INCLUDE ([registration_id],[comm_paid],[recognise]);
	CREATE NONCLUSTERED INDEX Idx2 ON [dbo].[sps_person] ([sps_person_id])
	CREATE NONCLUSTERED INDEX Idx3 ON [dbo].[persons_in_transaction]([transaction_guid],[person_roll]) INCLUDE ([person_id])

SELECT     
				  RIGHT(CONVERT(VARCHAR(10), sps_transaction.sps_listed_date, 105), 7) AS [Year_Month_Listed_On]
				, sps_transaction.sps_transaction_division AS [Division]
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
				, ISNULL(sps_transaction.sps_comm_amount,0) + ISNULL(sps_transaction.sps_rental_management_fee,0) AS [Total Managed Non Managed Fee]
				, (ISNULL(sps_transaction.sps_comm_amount,0) + ISNULL(sps_transaction.sps_rental_management_fee,0))/0.07 AS [Total Managed Non Managed Fee AT 7 Percent]
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
				license.license_id = @LicenseID				
AND 
				(sps_transaction.sps_listed_date > = @Begin) 
AND 
        (sps_transaction.sps_listed_date < = @End)
      
ORDER BY 
				RIGHT(CONVERT(VARCHAR(10), sps_transaction.sps_listed_date, 105), 7) ASC
				
				DROP INDEX Idx1 ON [dbo].[sps_agent_split] 
				DROP INDEX Idx2 ON [dbo].[sps_person]
				DROP INDEX Idx3 ON [dbo].[persons_in_transaction]
END

