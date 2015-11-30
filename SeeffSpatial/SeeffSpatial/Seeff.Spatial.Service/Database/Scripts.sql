
-- Adding area_name column to spatial_area:
-----------------------------------------------------------------------------------------------------------------------------------------------
alter table spatial_area
add area_name varchar(255)  null;
-- update from seeff.com DB.
update sss
set sss.area_name = a.areaName
from [seeff_spatial_staging].dbo.spatial_area sss
join [41.222.226.215].seeff.dbo.area a on a.areaId = sss.fkAreaId
-----------------------------------------------------------------------------------------------------------------------------------------------


-- spatial take-on from LS_BASE:
-----------------------------------------------------------------------------------------------------------------------------------------------
truncate table [seeff_spatial_staging].[dbo].[spatial_area];
truncate table [seeff_spatial_staging].[dbo].[spatial_license];
truncate table [seeff_spatial_staging].[dbo].[spatial_terretory];

insert into [seeff_spatial_staging].[dbo].[spatial_area] ([fkAreaId], [geo_polygon], [fk_license_id], [fk_territory_id], [area_name])
select [fkAreaId], [geo_polygon], [fk_license_id], [fk_territory_id], null
from ls_base.dbo.spatial_area;

insert into [seeff_spatial_staging].[dbo].[spatial_license] ([fk_license_id], [geo_polygon], [region], [fk_territory_id])
select [fk_license_id], [geo_polygon], [region], [fk_territory_id]
from ls_base.dbo.spatial_license;

set identity_insert [seeff_spatial_staging].[dbo].[spatial_terretory] ON

insert into [seeff_spatial_staging].[dbo].[spatial_terretory] ([territory_id], [territory_name], [geo_polygon])
select [territory_id], [territory_name], [geo_polygon]
from ls_base.[dbo].[spatial_terretory];

set identity_insert [seeff_spatial_staging].[dbo].[spatial_terretory] OFF
-----------------------------------------------------------------------------------------------------------------------------------------------