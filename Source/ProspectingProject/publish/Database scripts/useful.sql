
/*///////////////////////////////////////////////////////////////////////////////////////////////////////*/
//  Script to re-reprospect a sectional scheme safely. First make sure you are targeting the right SS (ie could be more than 1)
/*///////////////////////////////////////////////////////////////////////////////////////////////////////*/
delete pr from dbo.prospecting_person_property_relationship pr
join prospecting_property pp on pp.prospecting_property_id = pr.prospecting_property_id
where ss_name = 'SS AQUILA' and property_address = 'GRANITE ROAD, FOURWAYS, CITY OF JOHANNESBURG' and street_or_unit_no = '28'

delete cpr from [dbo].[prospecting_company_property_relationship] cpr
join prospecting_property pp on pp.prospecting_property_id = cpr.prospecting_property_id
where ss_name = 'SS AQUILA' and property_address = 'GRANITE ROAD, FOURWAYS, CITY OF JOHANNESBURG' and street_or_unit_no = '28'

alter table dbo.service_enquiry_log nocheck constraint all

ALTER TABLE [dbo].[activity_log] NOCHECK CONSTRAINT FK_lightstone_property_id

delete from prospecting_property where ss_name = 'SS AQUILA' and property_address = 'GRANITE ROAD, FOURWAYS, CITY OF JOHANNESBURG' and street_or_unit_no = '28'

ALTER TABLE  [dbo].[activity_log]CHECK CONSTRAINT FK_lightstone_property_id

alter table  dbo.service_enquiry_log check constraint all
/*///////////////////////////////////////////////////////////////////////////////////////////////////////*/

// Paul Kruger query to remove "ownerless" properties
//////////////////////////////////////////////////////////////////////////////////////////////////////////
ALTER TABLE [dbo].service_enquiry_log NOCHECK CONSTRAINT FK__prospecti__prosp__49C3F6B7
ALTER TABLE [dbo].[activity_log] NOCHECK CONSTRAINT FK_lightstone_property_id

delete pp from seeff_prospecting.dbo.prospecting_property pp
WHERE NOT EXISTS (
    SELECT *
    FROM prospecting_person_property_relationship pr
    WHERE pr.prospecting_property_id = pp.prospecting_property_id
) AND NOT EXISTS (
	select * from prospecting_company_property_relationship cpr
	where cpr.prospecting_property_id = pp.prospecting_property_id
) AND
(pp.ss_fh = 'FH' or pp.ss_name is null)
AND
pp.latest_reg_date is NULL
AND not linked to activities
AND
pp.seeff_area_id in (4479, 4480, 2247, 4481, 4483, 4484, 4485, 4486, 4493, 4487, 4488, 4489, 4503, 4490, 4491, 4482, 4492, 2248, 4494, 4495, 4496, 4497, 4498, 4499, 4500, 4501, 4502, 4504, 4505, 4506, 4508, 4509, 4510, 4511, 3151, 4512, 4423, 1295, 1301, 1298, 1299, 4424, 1294, 4425, 4426, 1303, 4427, 1309, 1302, 1293, 2305, 4428, 1306, 1300, 1304, 1296, 2306, 1305, 917, 2307, 4429, 4430, 4431, 4432, 4433, 4434, 1297, 2309, 2308, 1308, 4435, 1307);

ALTER TABLE  [dbo].service_enquiry_log CHECK CONSTRAINT FK__prospecti__prosp__49C3F6B7
ALTER TABLE  [dbo].[activity_log]CHECK CONSTRAINT FK_lightstone_property_id
//////////////////////////////////////////////////////////////////////////////////////////////////////////