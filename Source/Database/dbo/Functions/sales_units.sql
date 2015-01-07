
-- =============================================
-- Author:		Adam Roberts
-- Create date: 2012/01/30
-- Description:	The return no. of units.
-- =============================================
CREATE FUNCTION [dbo].[sales_units] 
(
	-- Add the parameters for the function here
	 @referral_type VARCHAR(100)
)
RETURNS INT
AS
BEGIN
	-- Declare the return variable here
	DECLARE @units INT;
    
    IF @referral_type = 'External paid to you'
      BEGIN
        SET @units = 0;
      END
    ELSE
      SET @units = 1;

	-- Return the result of the function
	RETURN @units
END








