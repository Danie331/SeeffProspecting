
-- =============================================
-- Author:		Michael Scott
-- Create date: 28/02/2013
-- Description:	Smart Account Cancellation Report for Mandy
-- =============================================
CREATE PROCEDURE [dbo].[spMIS_Smart_Account_Cancellation_Report] 
	-- Add the parameters for the stored procedure here

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
    
    SELECT     smart_acc_requests.smart_acc_name + ' ' + smart_acc_requests.smart_acc_surname AS [Account Holder], smart_acc_requests.smart_acc_email AS [Email Address], 
                      user_registration.user_preferred_name + ' ' + user_registration.user_surname AS [Deleted by], user_registration.user_email_address AS Email, 
                      user_registration.user_cellphone_no AS Cell, user_registration.user_office_no AS [Office No.], license.license_name AS [License]
FROM         smart_acc_requests INNER JOIN
                      license ON smart_acc_requests.license_id = license.license_id INNER JOIN
                      user_registration ON smart_acc_requests.audit_registration_id = user_registration.registration_id
WHERE     (smart_acc_requests.renew_account LIKE 'N')
ORDER BY license.license_name
	
END
