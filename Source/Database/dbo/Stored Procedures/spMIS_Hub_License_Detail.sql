

-- =============================================
-- Author:		GW Swanepoel
-- Create date: 2012-07-25
-- Description:	Returns a distinct list
--				of the licenses that have
--				Hub records
--				Used in the License Hub 
--				Transaction Status Report
-- =============================================
CREATE PROCEDURE [dbo].[spMIS_Hub_License_Detail]
	-- Add the parameters for the stored procedure here
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT     DISTINCT
			 license.license_id AS [License ID]
			,license.license_name AS [License]
			,license.license_name + ' - ' + license.Status AS LicenseNameStatus
			,license.region AS [Region]
FROM         
			sps_transaction AS sps_tran 
INNER JOIN
			sps_hub_transaction AS sps_hub_tran ON sps_tran.sps_transaction_ref = sps_hub_tran.sps_transaction_ref 
INNER JOIN
			user_registration AS user_registration_1 ON sps_hub_tran.registration_id = user_registration_1.registration_id 
INNER JOIN
			license_branches ON user_registration_1.branch_id = license_branches.branch_id 
INNER JOIN
			license ON license_branches.license_id = license.license_id
WHERE 
			(transaction_status NOT LIKE 'ARCHIVED')
AND
			(license.license_id NOT IN (107,109))
END


