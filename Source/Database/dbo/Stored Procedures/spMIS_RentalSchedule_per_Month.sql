

-- =============================================
-- Author:		Gustav Swanepoel
-- Create date: 03 October 2011
-- Description:	Stored procedure for RENTAL SCHEDULE
--							From Adam Robert Query
--							Pass month and year variables (INT)
--							to return resultset used currently
--							for the Rental Schedule Report
-- =============================================
CREATE PROCEDURE [dbo].[spMIS_RentalSchedule_per_Month]
	-- Add the parameters for the stored procedure here
	 @Month INT 
	,@Year	INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT     
/*A*/				sps_transaction.sps_transaction_division AS [Division]
/*B*/				, branch.branchName AS [Branch]
/*C*/				, dbo.credited_schedule_agents(sps_transaction.sps_transaction_ref) AS [Agent]
/*D*/				, sps_transaction.sps_lic_deal_no AS [Deal No.]
/*E*/				, sps_rental_renewal.sps_renewal_amount AS [Schedule Payment]
/*F*/				, sps_rental_renewal.sps_year_month AS [Schedule Date]
/*G*/				, sps_transaction.sps_listing_price AS [Monthly Rental]
/*H*/				, sps_transaction.sps_listed_date AS [Start Date]
/*I*/				, sps_transaction.sps_sold_date AS [End Date]
/*J*/				, sps_transaction.sps_property_address AS [Address]
/*K*/				, user_registration.user_preferred_name + ' ' + user_registration.user_surname AS [Captured By]
/*L*/				, license.license_name AS [License]
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
				(YEAR(sps_rental_renewal.sps_year_month) = @Year) 
AND 
        (MONTH(sps_rental_renewal.sps_year_month) = @Month)
ORDER BY 
				sps_transaction.sps_transaction_type
				, sps_transaction.sps_reporting_date
END


