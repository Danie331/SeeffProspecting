

-- =============================================
-- Author:		Gustav Swanepoel
-- Create date: <Create Date,,>
-- Description:	Runs the OtherIncome Report query
-- =============================================
CREATE PROCEDURE [dbo].[spMIS_OtherIncome]
	-- Add the parameters for the stored procedure here
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT     license.license_name, 
           CONVERT(varchar, sps_other_income.reporting_year) + '-' + CONVERT(varchar, sps_other_income.reporting_month) AS Reporting_Month, 
           sps_other_income.reported_amount, 
           sps_other_income.description, 
           user_registration.user_preferred_name + ' ' + user_registration.user_surname AS Reported_By, 
           user_registration.user_email_address
FROM         sps_other_income INNER JOIN
                      license ON sps_other_income.license_id = license.license_id INNER JOIN
                      user_registration ON sps_other_income.reported_by = user_registration.registration_id
WHERE license.license_id NOT IN (109)
ORDER BY Reporting_Month 
END


