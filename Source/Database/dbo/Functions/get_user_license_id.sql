



-- =============================================
-- Author:		Adam Roberts
-- Create date: 2011/11/20
-- Description:	Calculates the company sales commission
-- =============================================
CREATE FUNCTION [dbo].[get_user_license_id] 
(
	-- Add the parameters for the function here
	 @registration_id INT
)
RETURNS INT
AS
BEGIN
	-- Declare the return variable here
	DECLARE @license_id INT;
    
    SET @license_id = (SELECT license.license_id
                       FROM   user_registration INNER JOIN
                      license_branches ON user_registration.branch_id = license_branches.branch_id INNER JOIN
                      license ON license_branches.license_id = license.license_id
                      WHERE user_registration.registration_id = @registration_id);
      
	-- Return the result of the function
	RETURN @license_id
END










