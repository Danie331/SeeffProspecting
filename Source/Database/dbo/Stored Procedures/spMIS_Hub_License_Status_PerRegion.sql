


-- =============================================
-- Author:		GW Swanepoel
-- Create date: 2012-07-25
-- Description:	Returns distinct list of statuses
--				per license
-- =============================================
CREATE PROCEDURE [dbo].[spMIS_Hub_License_Status_PerRegion] 
	-- Add the parameters for the stored procedure here
	@Region NVARCHAR(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT DISTINCT
	
				CASE sps_hub_tran.transaction_status
					WHEN 'Rev Ref Request' THEN 2 
					WHEN 'Loaded' THEN 3
					WHEN 'Accepted' THEN 4
					WHEN 'Property Active' THEN 5
					WHEN 'OTP Received' THEN 6
					WHEN 'OTP Accepted' THEN 7
					WHEN 'OTP Conditions Met' THEN 8
					WHEN 'Sold' THEN 9
					WHEN 'Invoiced' THEN 10
					WHEN 'Registered' THEN 11
					WHEN 'Declined' THEN 12
					WHEN 'Mandate Period Expired' THEN 13
					WHEN 'Withdrawn' THEN 14
					WHEN 'Expired' THEN 15
					WHEN 'Escalated - Lynne' THEN 16
					WHEN 'Escalated - Tracy' THEN 17
					WHEN 'Sale Fell Through' THEN 18
					ELSE 99
					END AS [Process Order]	 
					,sps_hub_tran.transaction_status  
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
			(license.region = @Region)			
UNION
	
	SELECT 1, '_ALL' 
	
	ORDER BY
[Process Order] ASC				
			
END



