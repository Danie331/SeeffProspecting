alter table prospecting_property
add
    has_email bit default(0),
	has_cell bit default(0),
	has_landline bit default(0),
	has_primary_email bit default(0),
	has_primary_cell bit default(0),
	has_primary_landline bit default(0);


alter procedure dbo.update_property_contact_detail_statistics (@area_id int)
--
as
begin
if @area_id is not null 
begin

update prospecting_property set has_email = 0, has_cell = 0, has_landline = 0, has_primary_email = 0, has_primary_cell = 0, has_primary_landline = 0
where seeff_area_id = @area_id;

update pp
set pp.has_email = 1					 																			 					 
from prospecting_property pp
join prospecting_company_property_relationship cpr on pp.prospecting_property_id = cpr.prospecting_property_id
join prospecting_person_company_relationship pcr on cpr.contact_company_id = pcr.contact_company_id
join prospecting_contact_person cp on pcr.contact_person_id = cp.contact_person_id
join prospecting_contact_detail cd on cp.contact_person_id = cd.contact_person_id
where cd.deleted = 0 and cd.contact_detail_type in (4,5)
and pp.seeff_area_id = @area_id;

update pp
set pp.has_cell = 1					 																			 					 
from prospecting_property pp
join prospecting_company_property_relationship cpr on pp.prospecting_property_id = cpr.prospecting_property_id
join prospecting_person_company_relationship pcr on cpr.contact_company_id = pcr.contact_company_id
join prospecting_contact_person cp on pcr.contact_person_id = cp.contact_person_id
join prospecting_contact_detail cd on cp.contact_person_id = cd.contact_person_id
where cd.deleted = 0 and cd.contact_detail_type = 3
and pp.seeff_area_id = @area_id;

update pp
set pp.has_landline = 1					 																			 					 
from prospecting_property pp
join prospecting_company_property_relationship cpr on pp.prospecting_property_id = cpr.prospecting_property_id
join prospecting_person_company_relationship pcr on cpr.contact_company_id = pcr.contact_company_id
join prospecting_contact_person cp on pcr.contact_person_id = cp.contact_person_id
join prospecting_contact_detail cd on cp.contact_person_id = cd.contact_person_id
where cd.deleted = 0 and cd.contact_detail_type in (1,2)
and pp.seeff_area_id = @area_id;

update pp
set pp.has_primary_email = 1					 																			 					 
from prospecting_property pp
join prospecting_company_property_relationship cpr on pp.prospecting_property_id = cpr.prospecting_property_id
join prospecting_person_company_relationship pcr on cpr.contact_company_id = pcr.contact_company_id
join prospecting_contact_person cp on pcr.contact_person_id = cp.contact_person_id
join prospecting_contact_detail cd on cp.contact_person_id = cd.contact_person_id
where cd.deleted = 0 and cd.contact_detail_type in (4,5) and cd.is_primary_contact = 1
and pp.seeff_area_id = @area_id;

update pp
set pp.has_primary_cell = 1					 																			 					 
from prospecting_property pp
join prospecting_company_property_relationship cpr on pp.prospecting_property_id = cpr.prospecting_property_id
join prospecting_person_company_relationship pcr on cpr.contact_company_id = pcr.contact_company_id
join prospecting_contact_person cp on pcr.contact_person_id = cp.contact_person_id
join prospecting_contact_detail cd on cp.contact_person_id = cd.contact_person_id
where cd.deleted = 0 and cd.contact_detail_type = 3 and cd.is_primary_contact = 1
and pp.seeff_area_id = @area_id;

update pp
set pp.has_primary_landline = 1					 																			 					 
from prospecting_property pp
join prospecting_company_property_relationship cpr on pp.prospecting_property_id = cpr.prospecting_property_id
join prospecting_person_company_relationship pcr on cpr.contact_company_id = pcr.contact_company_id
join prospecting_contact_person cp on pcr.contact_person_id = cp.contact_person_id
join prospecting_contact_detail cd on cp.contact_person_id = cd.contact_person_id
where cd.deleted = 0 and cd.contact_detail_type in (1,2) and cd.is_primary_contact = 1
and pp.seeff_area_id = @area_id;
--

update pp
set pp.has_email = 1					 																			 					 
from prospecting_property pp
join prospecting_person_property_relationship ppr on ppr.prospecting_property_id = pp.prospecting_property_id
join prospecting_contact_person cp on ppr.contact_person_id = cp.contact_person_id
join prospecting_contact_detail cd on cp.contact_person_id = cd.contact_person_id
where cd.deleted = 0 and cd.contact_detail_type in (4,5)
and pp.seeff_area_id = @area_id;

update pp
set pp.has_cell = 1					 																			 					 
from prospecting_property pp
join prospecting_person_property_relationship ppr on ppr.prospecting_property_id = pp.prospecting_property_id
join prospecting_contact_person cp on ppr.contact_person_id = cp.contact_person_id
join prospecting_contact_detail cd on cp.contact_person_id = cd.contact_person_id
where cd.deleted = 0 and cd.contact_detail_type = 3
and pp.seeff_area_id = @area_id;

update pp
set pp.has_landline = 1					 																			 					 
from prospecting_property pp
join prospecting_person_property_relationship ppr on ppr.prospecting_property_id = pp.prospecting_property_id
join prospecting_contact_person cp on ppr.contact_person_id = cp.contact_person_id
join prospecting_contact_detail cd on cp.contact_person_id = cd.contact_person_id
where cd.deleted = 0 and cd.contact_detail_type in (1,2)
and pp.seeff_area_id = @area_id;

update pp
set pp.has_primary_email = 1					 																			 					 
from prospecting_property pp
join prospecting_person_property_relationship ppr on ppr.prospecting_property_id = pp.prospecting_property_id
join prospecting_contact_person cp on ppr.contact_person_id = cp.contact_person_id
join prospecting_contact_detail cd on cp.contact_person_id = cd.contact_person_id
where cd.deleted = 0 and cd.contact_detail_type in (4,5) and cd.is_primary_contact = 1
and pp.seeff_area_id = @area_id;

update pp
set pp.has_primary_cell = 1					 																			 					 
from prospecting_property pp
join prospecting_person_property_relationship ppr on ppr.prospecting_property_id = pp.prospecting_property_id
join prospecting_contact_person cp on ppr.contact_person_id = cp.contact_person_id
join prospecting_contact_detail cd on cp.contact_person_id = cd.contact_person_id
where cd.deleted = 0 and cd.contact_detail_type = 3 and cd.is_primary_contact = 1
and pp.seeff_area_id = @area_id;

update pp
set pp.has_primary_landline = 1					 																			 					 
from prospecting_property pp
join prospecting_person_property_relationship ppr on ppr.prospecting_property_id = pp.prospecting_property_id
join prospecting_contact_person cp on ppr.contact_person_id = cp.contact_person_id
join prospecting_contact_detail cd on cp.contact_person_id = cd.contact_person_id
where cd.deleted = 0 and cd.contact_detail_type in (1,2) and cd.is_primary_contact = 1
and pp.seeff_area_id = @area_id;
					 																			 					 
end
else 
begin
	update prospecting_property set has_email = 0, has_cell = 0,has_landline = 0, has_primary_email = 0,has_primary_cell = 0,has_primary_landline = 0;

update pp
set pp.has_email = 1					 																			 					 
from prospecting_property pp
join prospecting_company_property_relationship cpr on pp.prospecting_property_id = cpr.prospecting_property_id
join prospecting_person_company_relationship pcr on cpr.contact_company_id = pcr.contact_company_id
join prospecting_contact_person cp on pcr.contact_person_id = cp.contact_person_id
join prospecting_contact_detail cd on cp.contact_person_id = cd.contact_person_id
where cd.deleted = 0 and cd.contact_detail_type in (4,5);

update pp
set pp.has_cell = 1					 																			 					 
from prospecting_property pp
join prospecting_company_property_relationship cpr on pp.prospecting_property_id = cpr.prospecting_property_id
join prospecting_person_company_relationship pcr on cpr.contact_company_id = pcr.contact_company_id
join prospecting_contact_person cp on pcr.contact_person_id = cp.contact_person_id
join prospecting_contact_detail cd on cp.contact_person_id = cd.contact_person_id
where cd.deleted = 0 and cd.contact_detail_type = 3;

update pp
set pp.has_landline = 1					 																			 					 
from prospecting_property pp
join prospecting_company_property_relationship cpr on pp.prospecting_property_id = cpr.prospecting_property_id
join prospecting_person_company_relationship pcr on cpr.contact_company_id = pcr.contact_company_id
join prospecting_contact_person cp on pcr.contact_person_id = cp.contact_person_id
join prospecting_contact_detail cd on cp.contact_person_id = cd.contact_person_id
where cd.deleted = 0 and cd.contact_detail_type in (1,2);

update pp
set pp.has_primary_email = 1					 																			 					 
from prospecting_property pp
join prospecting_company_property_relationship cpr on pp.prospecting_property_id = cpr.prospecting_property_id
join prospecting_person_company_relationship pcr on cpr.contact_company_id = pcr.contact_company_id
join prospecting_contact_person cp on pcr.contact_person_id = cp.contact_person_id
join prospecting_contact_detail cd on cp.contact_person_id = cd.contact_person_id
where cd.deleted = 0 and cd.contact_detail_type in (4,5) and cd.is_primary_contact = 1;

update pp
set pp.has_primary_cell = 1					 																			 					 
from prospecting_property pp
join prospecting_company_property_relationship cpr on pp.prospecting_property_id = cpr.prospecting_property_id
join prospecting_person_company_relationship pcr on cpr.contact_company_id = pcr.contact_company_id
join prospecting_contact_person cp on pcr.contact_person_id = cp.contact_person_id
join prospecting_contact_detail cd on cp.contact_person_id = cd.contact_person_id
where cd.deleted = 0 and cd.contact_detail_type = 3 and cd.is_primary_contact = 1;

update pp
set pp.has_primary_landline = 1					 																			 					 
from prospecting_property pp
join prospecting_company_property_relationship cpr on pp.prospecting_property_id = cpr.prospecting_property_id
join prospecting_person_company_relationship pcr on cpr.contact_company_id = pcr.contact_company_id
join prospecting_contact_person cp on pcr.contact_person_id = cp.contact_person_id
join prospecting_contact_detail cd on cp.contact_person_id = cd.contact_person_id
where cd.deleted = 0 and cd.contact_detail_type in (1,2) and cd.is_primary_contact = 1;
--

update pp
set pp.has_email = 1					 																			 					 
from prospecting_property pp
join prospecting_person_property_relationship ppr on ppr.prospecting_property_id = pp.prospecting_property_id
join prospecting_contact_person cp on ppr.contact_person_id = cp.contact_person_id
join prospecting_contact_detail cd on cp.contact_person_id = cd.contact_person_id
where cd.deleted = 0 and cd.contact_detail_type in (4,5);

update pp
set pp.has_cell = 1					 																			 					 
from prospecting_property pp
join prospecting_person_property_relationship ppr on ppr.prospecting_property_id = pp.prospecting_property_id
join prospecting_contact_person cp on ppr.contact_person_id = cp.contact_person_id
join prospecting_contact_detail cd on cp.contact_person_id = cd.contact_person_id
where cd.deleted = 0 and cd.contact_detail_type = 3;

update pp
set pp.has_landline = 1					 																			 					 
from prospecting_property pp
join prospecting_person_property_relationship ppr on ppr.prospecting_property_id = pp.prospecting_property_id
join prospecting_contact_person cp on ppr.contact_person_id = cp.contact_person_id
join prospecting_contact_detail cd on cp.contact_person_id = cd.contact_person_id
where cd.deleted = 0 and cd.contact_detail_type in (1,2);

update pp
set pp.has_primary_email = 1					 																			 					 
from prospecting_property pp
join prospecting_person_property_relationship ppr on ppr.prospecting_property_id = pp.prospecting_property_id
join prospecting_contact_person cp on ppr.contact_person_id = cp.contact_person_id
join prospecting_contact_detail cd on cp.contact_person_id = cd.contact_person_id
where cd.deleted = 0 and cd.contact_detail_type in (4,5) and cd.is_primary_contact = 1;

update pp
set pp.has_primary_cell = 1					 																			 					 
from prospecting_property pp
join prospecting_person_property_relationship ppr on ppr.prospecting_property_id = pp.prospecting_property_id
join prospecting_contact_person cp on ppr.contact_person_id = cp.contact_person_id
join prospecting_contact_detail cd on cp.contact_person_id = cd.contact_person_id
where cd.deleted = 0 and cd.contact_detail_type = 3 and cd.is_primary_contact = 1;

update pp
set pp.has_primary_landline = 1					 																			 					 
from prospecting_property pp
join prospecting_person_property_relationship ppr on ppr.prospecting_property_id = pp.prospecting_property_id
join prospecting_contact_person cp on ppr.contact_person_id = cp.contact_person_id
join prospecting_contact_detail cd on cp.contact_person_id = cd.contact_person_id
where cd.deleted = 0 and cd.contact_detail_type in (1,2) and cd.is_primary_contact = 1;
end

end;