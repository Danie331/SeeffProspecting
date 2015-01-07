
-- =============================================
-- Author:		Michael Scott
-- Create date: 2014-02-26
-- Description:	Creates the Smart Academy Certificate Tracker
--				Nationally.
-- =============================================
CREATE PROCEDURE [dbo].[spMIS_Cert_Track_National] 
	-- Add the parameters for the stored procedure here

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

SELECT license.license_name AS 'License', [user_preferred_name] + ' ' + [user_surname] AS Name,  cert_name AS 'Certificate Name', cert_print AS 'Date Printed', cert_email AS 'Email Address'
  FROM certificate_track
  INNER JOIN user_registration ON certificate_track.registration_id = user_registration.registration_id
  INNER JOIN branch ON user_registration.branch_id = branch.branchId
  INNER JOIN license_branches ON branch.branchId = license_branches.branch_id
  INNER JOIN license ON license_branches.license_id = license.license_id
  WHERE user_registration.branch_id NOT IN (260, 263, 277)
  ORDER BY license.license_name, cert_print DESC  
			
END



