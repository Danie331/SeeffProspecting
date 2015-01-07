

-- =============================================
-- Author:		Adam Roberts
-- Create date: 2014-10-18
-- Description:	Using the spatial table return the AreaID 
--              of a Lat, Long pair
-- =============================================
CREATE PROCEDURE [dbo].[get_area_id] 
(
	-- Add the parameters for the function here
	@lat float,
	@long float
)
AS
BEGIN
	DECLARE @license_id int;
	EXEC @license_id = [ls_base].[dbo].[get_license_id] @lat, @long;
 
	DELETE FROM [dbo].[spatial_area_temp];

	INSERT INTO [dbo].[spatial_area_temp]
           ([fkAreaId]
           ,[geo_polygon]
           ,[fk_license_id]
           ,[kml_coords])
	SELECT [fkAreaId]
      ,[geo_polygon]
      ,[fk_license_id]
      ,[kml_coords]
	FROM [dbo].[spatial_area]
	WHERE [fkAreaId] IN (SELECT DISTINCT [fkAreaId]
						   FROM [seeff].[dbo].[branch_area]
						   WHERE fkBranchid IN (SELECT Branchid 
												  FROM [seeff].dbo.branch 
												 WHERE fk_license_id = @license_id));

	-- Declare the return variable here
	DECLARE @area_id int
	
	DECLARE @g geography; 
    SET @g = geography::Point(@lat, @long, 4326)

	-- Add the T-SQL statements to compute the return value here
	SELECT @area_id = [fkAreaId]
      FROM [dbo].[spatial_area_temp]
     WHERE [geo_polygon].Filter(@g) = 1;

	IF @area_id IS NULL
	   RETURN -1;
	 
	 RETURN @area_id;
END


