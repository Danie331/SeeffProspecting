
-- =============================================
-- Author:		GW Swanepoel
-- Create date: 20120709
-- Description:	See the tab on the Google sheet
--				ICAS Recon 20120707
--				Query written by Adam Roberts
-- =============================================
CREATE PROCEDURE [dbo].[spMIS_ICAS_Recon] 
	-- Add the parameters for the stored procedure here
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT   DISTINCT  
			  user_registration.user_preferred_name AS [User Preferred Name]
			, user_registration.user_surname AS [User Surname]
			--, license.license_name AS [License]
			, CAST(user_registration.icas AS Integer) AS [ICAS]
			, CASE
				WHEN user_registration.icas = 1 THEN 'Yes'
				WHEN user_registration.icas = 0 THEN 'No'
				ELSE ''
			  END AS [Subscriber]	
FROM         
			licensee 
INNER JOIN
			license ON licensee.license_id = license.license_id 
INNER JOIN
			user_registration ON licensee.registration_id = user_registration.registration_id
WHERE     
			(license.icas = 1)
ORDER BY 
			[ICAS]

END

