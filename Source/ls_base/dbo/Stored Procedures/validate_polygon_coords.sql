
CREATE procedure dbo.validate_polygon_coords (@poly_coords varchar(max))
as
begin
		DECLARE @geom geometry;
		declare @geog geography;
		begin try
			set @geom = geometry::STGeomFromText('POLYGON((' + @poly_coords + '))', 4236);
			SET @geom = @geom.MakeValid();
			SET @geog = GEOGRAPHY::STGeomFromText(@geom.STAsText(),4236);

			return 1; -- valid
		end try
		begin catch
			return 0; -- invalid
		end catch
end;