
-- =============================================
-- Author:		GW Swanepoel
-- Create date: 2012-06-29
-- Description:	Adam Roberts query in stored
--				procedure form.
--				Returns results for REBOSA
-- =============================================
CREATE PROCEDURE [dbo].[spMIS_REBOSA]
	-- Add the parameters for the stored procedure here
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT license.license_name AS [License], (user_registration.user_preferred_name+' '+user_registration.user_surname) AS [BOSS User]
  FROM user_registration INNER JOIN
            license_branches ON user_registration.branch_id = license_branches.branch_id INNER JOIN
            license ON license_branches.license_id = license.license_id
WHERE user_registration.designation IN ('Rental Agent', 'Agent')
  AND user_registration.user_email_address LIKE '%@seeff.com'
  AND user_registration.confirmation LIKE 'Y'
ORDER BY license.license_name, [BOSS User]
 
END

