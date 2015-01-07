
-- =============================================
-- Author:		Adam Roberts
-- Create date: 2014 11 02
-- Description:	Return territory name with id
-- =============================================
CREATE FUNCTION [dbo].[get_territory_name]
(
	-- Add the parameters for the function here
	@territory_id int
)
RETURNS varchar(500) 
AS
BEGIN
	-- Declare the return variable here
	DECLARE @result varchar(500)

	-- Add the T-SQL statements to compute the return value here
	SELECT @result = [territory_name]
    FROM [dbo].[spatial_terretory]
	WHERE [territory_id] = @territory_id

	-- Return the result of the function
	RETURN @result

END

