use ls_base
GO

create procedure dbo.sp_populate_area_layer_tbl
as
begin
	truncate table [dbo].[area_layer];

	declare @area_id int;
	declare @area_type varchar(1);
	declare @prov_id int;
	declare @poly_coords varchar(max) = '';
	declare @startPoint varchar(max);
	declare @lastCommaIndex int;

	DECLARE @geom geometry;
	declare @geog geography;

	declare area_area_type_cursor cursor for 
		SELECT [t0].[area_id], [t0].[area_type]
		FROM seeff.[dbo].[kml_area] AS [t0]
		GROUP BY [t0].[area_id], [t0].[area_type]
		order by [t0].[area_id], [t0].[area_type];

	OPEN area_area_type_cursor
	FETCH NEXT  FROM area_area_type_cursor INTO @area_id, @area_type
	WHILE @@FETCH_STATUS = 0
	BEGIN
		set @poly_coords = '';
		-- Get the polycoords as a comma-separated string of latitude longitudes. 
		select @poly_coords = @poly_coords + cast(latitude as varchar) + ' ' + cast(longitude as varchar) + ',' from seeff.dbo.kml_area
							where area_id = @area_id and area_type = @area_type;

		-- remove the last ',' appended to the end of the string						
		set @poly_coords = substring(@poly_coords, 0, len(@poly_coords) - 1);

		-- ensure that the coords are in a valid state, ie start position must equal end position
		set @startPoint = LEFT(@poly_coords,CHARINDEX(',',@poly_coords)-1);
		set @lastCommaIndex = LEN(@poly_coords) - CHARINDEX(',', REVERSE(@poly_coords)) + 1;
		set @poly_coords =  SUBSTRING(@poly_coords, 1, @lastCommaIndex);
		set @poly_coords = @poly_coords + @startPoint;

		-- find the province id of the area in question
		set @prov_id = seeff.dbo.GetAreaPathMap(@area_id, 2);

		-- before inserting the record we need to ensure that the "ring orientation" is valid
		begin try
			set @geom = geometry::STGeomFromText('POLYGON((' + @poly_coords + '))', 4236);
			SET @geom = @geom.MakeValid();
			SET @geog = GEOGRAPHY::STGeomFromText(@geom.STAsText(),4236);
		end try
		begin catch
			-- if an exception was thrown then reverse the order of the coords
			-- the logic above is the same logic used to determine if a point is inside a polygon
			print 'Reversing order of coordinates...';
			set @poly_coords = dbo.reverse_comma_delimited_string(@poly_coords);
		end catch

		-- finally insert a new record into area_layer
		insert into area_layer (area_id, area_type, province_id, formatted_poly_coords)
		values (@area_id, @area_type, @prov_id, @poly_coords);

		FETCH NEXT FROM area_area_type_cursor 
		INTO @area_id, @area_type
	END;

	CLOSE area_area_type_cursor;
	DEALLOCATE area_area_type_cursor;
end;