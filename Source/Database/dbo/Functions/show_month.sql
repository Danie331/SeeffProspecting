
-- =============================================
-- Author:		Adam Roberts
-- Create date: 2011/05/09
-- Description:	Return the month name from the number
-- =============================================
CREATE FUNCTION [dbo].[show_month] 
(
	-- Add the parameters for the function here
	@month_value INT
)
RETURNS VARCHAR(20)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @str_month VARCHAR(20);

	-- Add the T-SQL statements to compute the return value here
	SELECT   @str_month =
      CASE @month_value
         WHEN 1 THEN 'Jan'
         WHEN 2 THEN 'Feb'
         WHEN 3 THEN 'Mar'
         WHEN 4 THEN 'Apr'
         WHEN 5 THEN 'May'
         WHEN 6 THEN 'Jun'
         WHEN 7 THEN 'Jul'
         WHEN 8 THEN 'Aug'
         WHEN 9 THEN 'Sep'
         WHEN 10 THEN 'Oct'
         WHEN 11 THEN 'Nov'
         WHEN 12 THEN 'Dec'
         ELSE 'Not Set'
      END
	
	-- Return the result of the function
	RETURN @str_month
END

