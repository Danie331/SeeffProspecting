
-- =============================================
-- Author:		Gustav Swanepoel
-- Create date: 2012-02-15
-- Description:	Stored procedure to retrieve
--				OUTGOING referrals
-- =============================================
CREATE PROCEDURE [dbo].[spMIS_Referrals_OutGoing]
	-- Add the parameters for the stored procedure here
	@LicenseID as INTEGER
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT		  
				  sm_pa.smart_pass_id AS [Smart Pass Id]
				, sm_pa.referral_type AS [Referral Type]
				, CASE sm_pa.department WHEN 'S' THEN 'Sales' WHEN 'R' THEN 'Rentals' ELSE 'Error' END AS [Department]
				, sm_pa.expiry_date AS [Expiry Date]
				, sm_pa.current_status AS [Current Status]
				, [dbo].[last_smart_pass_comment] (sm_pa.smart_pass_id) AS [Last Comment]
				, sm_pa.created_date AS [Created Date ]
				, sm_pa.value_from AS [Value From]
				, sm_pa.value_to AS [Value To]
				, sm_pa.no_beds AS [Nr. Beds]
				, sm_pa.no_baths AS [Nr. Baths]
				, sm_pa.property_desc AS [Property Description]
				, sm_pa.property_geo_code AS [Property Geo Code]
				, sm_pa.division AS [Division]
				, sm_pa.category AS [Category]
				, sm_pa.area_desc AS [Area Description]
				, dbo.get_entity_desc(sm_pa.from_entity_type, sm_pa.from_entity_id)
                      AS [Agent From]
				, dbo.get_entity_desc(sm_pa.entity_type, sm_pa.entity_id) AS [Agent To]
                , CASE sm_pa.source WHEN 'A' THEN 'Pending Allocation' WHEN 'R' THEN 'Referred' ELSE 'Error' END AS [Branch Status]
                , smart_pass_person.smart_pass_title + ' ' + smart_pass_person.smart_pass_name + ' ' + smart_pass_person.smart_pass_surname AS [Contact]
                , smart_pass_person.smart_pass_contact_type + ': (+' + smart_pass_person.smart_pass_country_code + ') ' + smart_pass_person.smart_pass_contact_no AS [Contact Nr.]
                , smart_pass_person.smart_pass_email_address AS [Smart Pass Email Address]
                , license_1.license_name AS [License From]
                , user_registration_1.user_preferred_name + ' ' + user_registration_1.user_surname AS [User From]
                , license.license_name AS [License To]
                , user_registration.user_preferred_name + ' ' + user_registration.user_surname AS [User To]
                , smart_pass_lead.lead_desc AS [Lead]
FROM         
				smart_pass AS sm_pa
INNER JOIN
                smart_pass_participants ON sm_pa.smart_pass_id = smart_pass_participants.smart_pass_id 
INNER JOIN
                smart_pass_person ON smart_pass_participants.smart_pass_person_id = smart_pass_person.smart_pass_person_id 
INNER JOIN
                user_registration ON sm_pa.registration_id = user_registration.registration_id 
INNER JOIN
                license ON sm_pa.license_id_to = license.license_id 
INNER JOIN
                license AS license_1 ON sm_pa.license_id_from = license_1.license_id 
INNER JOIN
                user_registration AS user_registration_1 ON sm_pa.created_by = user_registration_1.registration_id
 LEFT OUTER JOIN
                smart_pass_lead ON sm_pa.lead_id = smart_pass_lead.lead_id
-- Outgoing Referrals: license_1.license_id As id_from
WHERE 
				license_1.license_id = @LicenseID
AND
				sm_pa.created_by <> sm_pa.registration_id				

END

