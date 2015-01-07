






-- =============================================
-- Author:		Adam Roberts
-- Create date: 2011/11/22
-- Description:	Returns the Area Desc from the area_id
-- =============================================
CREATE FUNCTION [dbo].[get_area_desc_from_area_id] 
(
	-- Add the parameters for the function here
	 @area_id INT,
	 @testing CHAR(1)
)
RETURNS VARCHAR(MAX)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @area_decs VARCHAR(MAX);
    
    IF @testing = 'Y' 
       BEGIN
          -- Comment out section A when testing, and uncomment Section B  
          -- Section A
          SELECT @area_decs = 'get_area_desc_from_area_id is set to live'
          /* SELECT @area_decs = (SELECT [areaName]
                                 FROM [seeff].[dbo].[area]
                                WHERE [areaId] = @area_id) */
       END
    ELSE
       BEGIN
          -- Section B
          -- SELECT @area_decs = 'get_area_desc_from_area_id is set to testing'
           SELECT @area_decs = (SELECT [areaName]
                                 FROM [41.222.226.215].[seeff].[dbo].[area]
                                WHERE [areaId] = @area_id) 
       END
       
	-- Return the result of the function
	RETURN @area_decs
END


















