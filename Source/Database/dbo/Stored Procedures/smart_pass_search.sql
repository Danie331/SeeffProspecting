
-- =============================================
-- Author:		Adam Roberts
-- Create date: 2014-03-12
-- Description:	Smart Pass Search
-- =============================================
CREATE PROCEDURE [dbo].[smart_pass_search] 
	@smart_pass_id varchar(250), 
	@referral_type varchar(250), 
	@current_status varchar(250),
	@contact_person varchar(250)
AS
BEGIN
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT smart_pass.smart_pass_id, 
       smart_pass.referral_type, 
	   smart_pass.current_status, 
	   smart_pass.created_date, 
	   smart_pass.value_from, 
	   smart_pass.value_to, 
       smart_pass.no_beds, 
	   smart_pass.no_baths, 
	   smart_pass.property_desc, 
	   smart_pass.division, 
	   smart_pass.category, 
	   smart_pass.area_desc, 
	   (smart_pass_person.smart_pass_name + ' ' + smart_pass_person.smart_pass_surname) AS contact_person, 
	   smart_pass_person.smart_pass_contact_no, 
	   smart_pass_person.smart_pass_email_address, 
	   smart_pass_person.smart_pass_id_no, 
	   license.license_name As License_to, 
       license_1.license_name AS license_from
FROM     smart_pass INNER JOIN
                  smart_pass_participants ON smart_pass.smart_pass_id = smart_pass_participants.smart_pass_id INNER JOIN
                  smart_pass_person ON smart_pass_participants.smart_pass_person_id = smart_pass_person.smart_pass_person_id INNER JOIN
                  license ON smart_pass.license_id_to = license.license_id INNER JOIN
                  license AS license_1 ON smart_pass.license_id_from = license_1.license_id
WHERE smart_pass.smart_pass_id LIKE @smart_pass_id
  AND smart_pass.referral_type LIKE @referral_type
  AND smart_pass.current_status LIKE @current_status
  AND smart_pass_person.smart_pass_name + '%' + smart_pass_person.smart_pass_surname LIKE @contact_person
END

