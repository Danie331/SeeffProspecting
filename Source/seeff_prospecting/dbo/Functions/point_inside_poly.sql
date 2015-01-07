CREATE function [dbo].[point_inside_poly](@point_lat decimal(13,8), @point_lng decimal(13,8), @polygon_coords varchar(max))
returns int
as
begin
 DECLARE @g geography;
 DECLARE @h geography;
 declare @a geometry;
 set @a = geometry::STGeomFromText('POLYGON((' + @polygon_coords + '))', 4236);
 SET @a = @a.MakeValid();

 SET @g = GEOGRAPHY::STGeomFromText(@a.STAsText(),4236);
 IF @g.EnvelopeAngle() >= 90
 BEGIN
  SET @g = @g.ReorientObject();   
 END;

 SET @h = geography::Point(@point_lng,@point_lat, 4236);
 return @g.STIntersects(@h);
end