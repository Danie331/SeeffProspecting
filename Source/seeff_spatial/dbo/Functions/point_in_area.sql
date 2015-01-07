


-- =============================================
-- Author:		Adam Roberts
-- Create date: 2014-10-18
-- Description:	Using the spatial table return the AreaID 
--              of a Lat, Long pair
-- =============================================
CREATE FUNCTION [dbo].[point_in_area] 
(
	-- Add the parameters for the function here
	@lat float,
	@long float,
	@area_id int
)
RETURNS int
AS
BEGIN
	-- Declare the return variable here
	
	DECLARE @g geography; 
    SET @g = geography::Point(@lat, @long, 4326)

	DECLARE @area geography; 
    
	-- Add the T-SQL statements to compute the return value here
	SELECT @area = geo_polygon
      FROM [dbo].[spatial_area]
     WHERE [fkAreaId] = @area_id
     
	 declare @result int;
	 SELECT @result = @area.STIntersects(@g);
	 
	 RETURN @result;
END



