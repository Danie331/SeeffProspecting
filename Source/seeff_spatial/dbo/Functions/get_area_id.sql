

-- =============================================
-- Author:		Adam Roberts
-- Create date: 2014-10-18
-- Description:	Using the spatial table return the AreaID 
--              of a Lat, Long pair
-- =============================================
CREATE FUNCTION [dbo].[get_area_id] 
(
	-- Add the parameters for the function here
	@lat float,
	@long float
)
RETURNS int
AS
BEGIN
	-- Declare the return variable here
	DECLARE @area_id int
	
	DECLARE @g geography; 
    SET @g = geography::Point(@lat, @long, 4326)

	-- Add the T-SQL statements to compute the return value here
	SELECT @area_id = [fkAreaId]
      FROM [dbo].[spatial_area]
     WHERE [geo_polygon].Filter(@g) = 1;

	IF @area_id IS NULL
	   RETURN -1;
	 
	 RETURN @area_id;
END


