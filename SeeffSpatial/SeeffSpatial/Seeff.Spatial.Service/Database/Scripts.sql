
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

-------------------------------------------------------------------RE-INDEX PROSPECTING SUBURB-------------------------------------------------
alter procedure dbo.reindex_prospecting_suburb (@area_id int)
as
begin
if @area_id > 0 
begin
	update spatial_area
	set under_maintenance = 1
	where fkAreaId = @area_id;

	update seeff_prospecting.dbo.prospecting_property
	set seeff_area_id = -1
	where seeff_area_id = @area_id;

	declare @territory_id int = (select top 1 fk_territory_id from dbo.spatial_area where fkAreaId = @area_id);

	create table dbo.#lat_long_collection
	(
		lat decimal(13, 8),
		lng decimal(13, 8),
		seeff_area_id int
	);

	with cte (latitude, longitude)
	as
	(
		select latitude, longitude from seeff_prospecting.dbo.prospecting_property
		where seeff_area_id = -1
		group by latitude, longitude	
	)
	insert into dbo.#lat_long_collection (lat, lng, seeff_area_id)
	select latitude, longitude, dbo.get_area_id(latitude, longitude, @territory_id) from cte;

	update pp
	set seeff_area_id = tmp.seeff_area_id
	from seeff_prospecting.dbo.prospecting_property pp
	join dbo.#lat_long_collection tmp on tmp.lat = pp.latitude and tmp.lng = pp.longitude;

	drop table dbo.#lat_long_collection;

	update spatial_area
	set under_maintenance = 0
	where fkAreaId = @area_id;
end
end
--------------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------- GET_AREA_ID ---------------------------------------------------------------------------------
ALTER FUNCTION [dbo].[get_area_id] 
(
	-- Add the parameters for the function here
	@lat float,
	@long float,
	@territory_id int
)
RETURNS int
AS
BEGIN
	DECLARE @area_id int
	
	DECLARE @g geography; 
    SET @g = geography::Point(@lat, @long, 4326)

	if @territory_id > -1 
	begin
		with cte (geo_polygon, area_id)
		as 
		(
			select geo_polygon, fkAreaId from dbo.spatial_area
			where fk_territory_id = @territory_id		
		)
		SELECT top 1 @area_id = area_id
		FROM cte
		WHERE [geo_polygon].Filter(@g) = 1; 
	end
	else 
	begin
		SELECT top 1 @area_id = [fkAreaId]
		FROM [dbo].[spatial_area]
		WHERE [geo_polygon].Filter(@g) = 1; 
	end

	IF @area_id IS NULL
	   RETURN -1;
	 
	 RETURN @area_id;
END
--------------------------------------------------------------------------------------------------------------------------------------------------