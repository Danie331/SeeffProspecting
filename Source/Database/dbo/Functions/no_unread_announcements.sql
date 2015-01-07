







-- =============================================
-- Author:		Adam Roberts
-- Create date: 2011/07/21
-- Description:	Return unread message Count
-- =============================================
CREATE FUNCTION [dbo].[no_unread_announcements] 
(
	-- Add the parameters for the function here
	@registration_id INT
)
RETURNS INT
AS
BEGIN
	DECLARE @unread INT 
	SELECT @unread = 0
    SELECT @unread = COUNT(announcements_track.announcements_id)
      FROM announcements_track INNER JOIN
                      user_registration ON announcements_track.registration_id = user_registration.registration_id INNER JOIN
                      license_branches ON user_registration.branch_id = license_branches.branch_id INNER JOIN
                      license ON license_branches.license_id = license.license_id
WHERE     (announcements_track.registration_id = @registration_id)
GROUP BY user_registration.user_preferred_name, user_registration.user_surname
 	RETURN @unread 
END








