
-- =============================================
-- Author:		Adam Roberts
-- Create date: 2013-11-05
-- Description:	Get Area Name
-- =============================================
CREATE FUNCTION [dbo].[ms_get_area_name] 
(
	-- Add the parameters for the function here
	@area_id int
)
RETURNS VARCHAR (100)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @ResultVar as varchar (100)

	-- Add the T-SQL statements to compute the return value here
	SELECT @ResultVar = (SELECT areaName
                           FROM seeff.dbo.area
                          WHERE (areaId =@area_id))
	-- Return the result of the function
	RETURN @ResultVar

END

