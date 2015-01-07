
-- =============================================
-- Author:		GW Swanepoel
-- Create date: 20120502
-- Description:	Returns Hub Dealmakers
-- =============================================
CREATE PROCEDURE [dbo].[spMIS_Hub_DealMakers]
	-- Add the parameters for the stored procedure here
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT  DISTINCT   
				user_registration_1.registration_id	
				,user_registration_1.user_preferred_name 
				,user_registration_1.user_surname 
				
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
				(sps_transaction.sps_reporting_date IS NOT NULL)
END

