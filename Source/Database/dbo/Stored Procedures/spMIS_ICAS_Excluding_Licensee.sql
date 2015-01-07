

-- =============================================
-- Author:		GW Swanepoel
-- Create date: 29 June 2012
-- Description:	Adam Robert query in 
--				stored procedure format
--				for ICAS Recon report
--				02 July: now excludes licensee
--				AND 
--				user_registration.designation 
--				NOT IN ('Licensee')			
-- =============================================
CREATE PROCEDURE [dbo].[spMIS_ICAS_Excluding_Licensee] 
	-- Add the parameters for the stored procedure here
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT 
			license.license_name AS [License]
			, user_registration.user_surname AS [BOSS User Surname]
			, user_registration.user_preferred_name AS [BOSS User Preferred Name]
			, (user_registration.user_preferred_name+' '+user_registration.user_surname) as [BOSS User]
	FROM 
			license 
	INNER JOIN
            license_branches ON license.license_id = license_branches.license_id 
	INNER JOIN
               user_registration ON license_branches.branch_id = user_registration.branch_id
	WHERE 
		user_registration.icas = 1
	AND 
		license.icas = 1
	AND 
		user_registration.user_email_address LIKE '%@seeff.com'
	AND 
		user_registration.confirmation LIKE 'Y'
	AND 
		user_registration.designation NOT IN ('Licensee')	
		
	ORDER BY 
		license.license_name, [BOSS User]
END


