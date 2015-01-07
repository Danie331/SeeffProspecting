
-- =============================================
-- Author:		Adam Roberts
-- Create date: 2011/11/22
-- Description:	Returns the License ID from a seeff area
-- =============================================
CREATE FUNCTION [dbo].[get_license_id_from_area_id] 
(
	-- Add the parameters for the function here
	 @area_id INT,
	 @testing CHAR(1)
)
RETURNS INT
AS
BEGIN
	-- Declare the return variable here
	DECLARE @license_id INT;
    DECLARE @branch_id INT;
    
    IF @area_id = 28 
	   BEGIN
		   -- Zimbabwe	
	       RETURN 27
       END
    IF @area_id = 1338 
	   BEGIN
		   -- Ceres	
	       RETURN 61
       END
    
    IF @area_id = 1335 
	   BEGIN
		   -- Tulbagh 
	       RETURN 63
       END

    IF @testing = 'Y' 
       BEGIN
          -- Comment out section A when testing, and uncomment Section B  
          -- Section A
          SELECT @branch_id = -1
          SELECT @license_id = -1
          
          -- Section B
          /* 
          SELECT @branch_id = (SELECT TOP 1 [fkBranchId]
                                 FROM [seeff].[dbo].[branch_area]
                                WHERE [fkAreaId] = @area_id)
          SELECT @license_id = (SELECT TOP 1 [license_id]
                                  FROM [boss].[dbo].[license_branches]
                                 WHERE [branch_id] = @branch_id)                         
          */                       
       END
    ELSE
       BEGIN
          -- Uncomment section A when testing, and comment out Section B  
          -- Section A
          -- SELECT @branch_id = -1
          -- SELECT @license_id = -1
          
          -- Section B
          
          SELECT @branch_id = (SELECT TOP 1 [fkBranchId]
                                 FROM [41.222.226.215].[seeff].[dbo].[branch_area]
                                WHERE [fkAreaId] = @area_id)
          SELECT @license_id = (SELECT TOP 1 [license_id]
                                  FROM [boss].[dbo].[license_branches]
                                 WHERE [branch_id] = @branch_id) 
                         
       END
       
	-- Return the result of the function
	RETURN @license_id
END












