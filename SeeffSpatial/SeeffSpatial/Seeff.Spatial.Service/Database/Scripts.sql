alter table spatial_area
add area_name varchar(255)  null;


update sa
set area_name = pa.area_name
from spatial_area sa
join seeff_prospecting.dbo.prospecting_area pa 
on pa.prospecting_area_id = sa.fkAreaId;
