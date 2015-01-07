
-- =============================================
-- Author:		Gustav Swanepoel
-- Create date: 2012-03-23
-- Description:	Returns DETAIL transactions listing
--				per license for
--				RENTAL schedule amounts 
--				loaded on database
-- =============================================
CREATE PROCEDURE [dbo].[spMIS_RentalSchedule_Detail] 
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
/*A*/				 RIGHT(CONVERT(VARCHAR(10), sps_rental_renewal.sps_year_month, 105), 7) AS [Year_Month_Renewal]
/*B*/			    ,sps_transaction.sps_transaction_division AS [Division]
/*C*/				, branch.branchName AS [Branch]
/*D*/				, dbo.credited_schedule_agents(sps_transaction.sps_transaction_ref) AS [Agent]
/*E*/				, sps_transaction.sps_lic_deal_no AS [Deal No.]
/*F*/				, sps_rental_renewal.sps_renewal_amount AS [Schedule Payment]
/*G*/				, sps_rental_renewal.sps_renewal_amount / 0.07 AS [Schedule Payment At 7 Perc]
/*H*/				, sps_rental_renewal.sps_year_month AS [Schedule Date]
/*I*/				, sps_transaction.sps_listing_price AS [Monthly Rental]
/*J*/				, sps_transaction.sps_listed_date AS [Start Date]
/*K*/				, sps_transaction.sps_sold_date AS [End Date]
/*L*/				, sps_transaction.sps_property_address AS [Address]
/*M*/				, user_registration.user_preferred_name + ' ' + user_registration.user_surname AS [Captured By]
/*N*/				, license.license_name AS [License]
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
INNER JOIN
        sps_rental_renewal ON sps_transaction.sps_transaction_ref = sps_rental_renewal.sps_transaction_ref
WHERE     
				(sps_transaction.sps_cancelled = 0) 
AND 
				(sps_transaction.sps_transaction_type = 'Rental')
AND
				(license.license_id = @LicenseID)				 	
AND 
				(sps_rental_renewal.sps_year_month > = @Begin) 
AND 
        (sps_rental_renewal.sps_year_month < = @End)
      
ORDER BY 
				RIGHT(CONVERT(VARCHAR(10), sps_transaction.sps_listed_date, 105), 7) ASC
				
				DROP INDEX Idx1 ON [dbo].[sps_agent_split] 
				DROP INDEX Idx2 ON [dbo].[sps_person]
				DROP INDEX Idx3 ON [dbo].[persons_in_transaction]
END

