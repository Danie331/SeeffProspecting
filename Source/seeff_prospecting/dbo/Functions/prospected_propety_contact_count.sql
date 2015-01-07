

-- =============================================
-- Author:		Adam Roberts
-- Create date: 2014-10-08
-- Description:	Return the number of contact details 
--              that have been captured against a contact      
-- =============================================
CREATE FUNCTION [dbo].[prospected_propety_contact_count]
(
	@prospecting_property_id integer
)
RETURNS integer
AS
BEGIN
	DECLARE @person_contact_count integer, @company_contact_count integer

	 SELECT @company_contact_count = COUNT(prospecting_property.prospecting_property_id)
       FROM            prospecting_property INNER JOIN
                         prospecting_company_property_relationship ON prospecting_property.prospecting_property_id = prospecting_company_property_relationship.prospecting_property_id INNER JOIN
                         prospecting_contact_company ON prospecting_company_property_relationship.contact_company_id = prospecting_contact_company.contact_company_id INNER JOIN
                         prospecting_person_company_relationship ON prospecting_contact_company.contact_company_id = prospecting_person_company_relationship.contact_company_id INNER JOIN
                         prospecting_contact_detail ON prospecting_person_company_relationship.contact_person_id = prospecting_contact_detail.contact_person_id
      WHERE prospecting_property.prospecting_property_id = @prospecting_property_id


     SELECT @person_contact_count =  COUNT(prospecting_property.prospecting_property_id)
       FROM            prospecting_property INNER JOIN
                         prospecting_person_property_relationship ON prospecting_property.prospecting_property_id = prospecting_person_property_relationship.prospecting_property_id INNER JOIN
                         prospecting_contact_detail ON prospecting_person_property_relationship.contact_person_id = prospecting_contact_detail.contact_person_id
      WHERE prospecting_property.prospecting_property_id = @prospecting_property_id
	  
	-- Return the result of the function
	RETURN (@person_contact_count + @company_contact_count)

END


