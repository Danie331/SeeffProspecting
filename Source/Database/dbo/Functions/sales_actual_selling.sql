







-- =============================================
-- Author:		Adam Roberts
-- Create date: 2011/05/27
-- Description:	The return value from this function is the selling price if the sale is 'External paid to you'
--              the selling price is reduced to reflect the licenses percentage.
-- =============================================
CREATE FUNCTION [dbo].[sales_actual_selling] 
(
	-- Add the parameters for the function here
	 @referral_type VARCHAR(100),
	 @referral_amount DECIMAL(18,2),
	 @commission DECIMAL(18,2),
	 @selling_price DECIMAL(18,2)
)
RETURNS DECIMAL(18,2)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @actual_selling_price DECIMAL(18,4);
    DECLARE @total_comm DECIMAL(18,4);
    DECLARE @perc DECIMAL(18,4);
    
    IF @referral_type = 'External paid to you'
      BEGIN
        SET @total_comm = ISNULL(@referral_amount, 0) + ISNULL(@commission, 0);
        SET @perc = ISNULL(@commission, 0) / @total_comm
        SET @actual_selling_price = ISNULL(@selling_price, 0) * @perc;
      END
    ELSE
      SET @actual_selling_price = ISNULL(@selling_price, 0);

	-- Return the result of the function
	RETURN @actual_selling_price
END








