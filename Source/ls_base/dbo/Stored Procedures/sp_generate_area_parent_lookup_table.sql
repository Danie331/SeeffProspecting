
CREATE procedure [dbo].[sp_generate_area_parent_lookup_table]
as
begin
	-- Every call to this stored proc regenerates the area_parent_lookup table
	IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'area_parent_lookup'))
		-- drop  
		drop table dbo.area_parent_lookup;

		-- create table	
	create table dbo.area_parent_lookup
	(
		area_parent_lookup_id int primary key identity(1,1),
		name varchar(max) not null,
		area_id int not null,
		area_type varchar(8) not null
	);


INSERT INTO [dbo].[area_parent_lookup]
           ([area_id]
		   ,[name]
           ,[area_type])
SELECT DISTINCT seeff.dbo.area.areaId,
				seeff.dbo.area.areaName + ', ' + seeff.dbo.GetAreaParentName(seeff.dbo.area.areaId) + ' (' + (
				(CASE
			     WHEN seeff.dbo.kml_area.area_type != ''
					THEN
						 CASE 
							WHEN seeff.dbo.kml_area.area_type = 'R'
							THEN 'Residential'
							WHEN seeff.dbo.kml_area.area_type = 'C'
							THEN 'Commercial'
							WHEN seeff.dbo.kml_area.area_type = 'A'
							THEN 'Agricultural'
							ELSE 'Unmapped'
						  END
					ELSE 'Unmapped'
				 END
				)) + ')',
				seeff.dbo.kml_area.area_type
  FROM seeff.dbo.area
  JOIN seeff.dbo.kml_area ON seeff.dbo.area.areaId = seeff.dbo.kml_area.area_id

INSERT INTO [dbo].[area_parent_lookup]
           ([area_id]
		   ,[name]
           ,[area_type])
SELECT DISTINCT seeff.dbo.area.areaId,
				seeff.dbo.area.areaName + ', ' + seeff.dbo.GetAreaParentName(seeff.dbo.area.areaId) + ' (Unmapped)',
				'Unmapped'
  FROM seeff.dbo.area
  WHERE seeff.dbo.area.areaId NOT IN (SELECT seeff.dbo.kml_area.area_id FROM seeff.dbo.kml_area)
end