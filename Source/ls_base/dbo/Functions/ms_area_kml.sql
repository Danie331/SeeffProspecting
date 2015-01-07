

-- =============================================
-- Author:		Adam Roberts
-- Create date: 2013 10 30 
-- Description:	Return the KML coordinates for an area
-- =============================================
CREATE FUNCTION [dbo].[ms_area_kml]
(	
	-- Add the parameters for the function here
	@area_id int, 
	@area_type char
)
RETURNS TABLE 
AS
RETURN 
(
	--ms_area_kml
SELECT [latitude]
      ,[longitude]
      ,[seq]
  FROM [seeff].[dbo].[kml_area]
  WHERE [area_id] = @area_id
    AND [area_type] LIKE @area_type  
)


