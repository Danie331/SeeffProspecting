
-- =============================================
-- Author:		Adam Roberts
-- Create date: 2014/10/14
-- Description:	Returns the License ID from a seeff area
-- =============================================
CREATE FUNCTION [dbo].[license_id_from_area_id] 
(
	 @area_id int
)
RETURNS int
AS
BEGIN
	-- Declare the return variable here
	DECLARE @license_id int;
    DECLARE @branch_id int;
    
    SELECT @branch_id = (SELECT TOP 1 [fkBranchId]
                           FROM [seeff].[dbo].[branch_area]
                          WHERE [fkAreaId] = @area_id)
    SELECT @license_id = (SELECT TOP 1 [license_id]
                            FROM [41.222.226.213].[boss].[dbo].[license_branches]
                           WHERE [branch_id] = @branch_id) 

						   
	-- Return the result of the function
	RETURN @license_id
END













