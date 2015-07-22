
/*///////////////////////////////////////////////////////////////////////////////////////////////////////*/
//  Script to re-reprospect a sectional scheme safely. First make sure you are targeting the right SS (ie could be more than 1)
/*///////////////////////////////////////////////////////////////////////////////////////////////////////*/
delete pr from dbo.prospecting_person_property_relationship pr
join prospecting_property pp on pp.prospecting_property_id = pr.prospecting_property_id
where pp.ss_name = 'SS BIRCHFIELD'  --67 rows

delete cpr from [dbo].[prospecting_company_property_relationship] cpr
join prospecting_property pp on pp.prospecting_property_id = cpr.prospecting_property_id
where pp.ss_name = 'SS BIRCHFIELD'

delete from dbo.service_enquiry_log where [prospecting_property_id] in (select [prospecting_property_id] from prospecting_property where ss_name = 'SS HAZELMERE')

ALTER TABLE [dbo].[activity_log] NOCHECK CONSTRAINT FK_lightstone_property_id

delete from prospecting_property where ss_name = 'SS BIRCHFIELD' -- 55 rows

ALTER TABLE  [dbo].[activity_log]CHECK CONSTRAINT FK_lightstone_property_id
/*///////////////////////////////////////////////////////////////////////////////////////////////////////*/