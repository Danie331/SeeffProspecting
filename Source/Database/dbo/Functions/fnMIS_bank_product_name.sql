
-- =============================================
-- Author:		Gustav Swanepoel
-- Create date: 2012-03-16
-- Description:	Function that returns
--				the bank and product name
-- =============================================
CREATE FUNCTION [dbo].[fnMIS_bank_product_name]
(
	-- Add the parameters for the function here
	@bankName NVARCHAR(MAX)
	,@productName NVARCHAR(MAX)
)
RETURNS NVARCHAR(MAX)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @Result NVARCHAR(MAX)

	-- Add the T-SQL statements to compute the return value here
	SELECT @Result = @bankName + ' ' + @productName

	-- Return the result of the function
	RETURN @Result

END

