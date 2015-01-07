







-- =============================================
-- Author:		Adam Roberts
-- Create date: 2011/05/27
-- Description:	Calculates the percentage that the sale was done at. 
-- =============================================
CREATE FUNCTION [dbo].[sales_comm_perc] 
(
	-- Add the parameters for the function here
	 @referral_type VARCHAR(100),
	 @referral_amount DECIMAL(18,4),
	 @commission DECIMAL(18,4),
	 @selling_price DECIMAL(18,4)
)
RETURNS DECIMAL(18,4)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @perc DECIMAL(18,4);
    DECLARE @total_comm DECIMAL(18,4);
    
    SET @total_comm = 
      CASE @referral_type
         WHEN 'None' THEN ISNULL(@commission, 0)
         WHEN 'Inter-license' THEN ISNULL(@commission, 0) + ISNULL(@referral_amount, 0)
         WHEN 'Internal' THEN ISNULL(@commission, 0) + ISNULL(@referral_amount, 0)
         WHEN 'External you paid' THEN ISNULL(@commission, 0) + ISNULL(@referral_amount, 0)
         WHEN 'External paid to you' THEN ISNULL(@commission, 0) + ISNULL(@referral_amount, 0)
         ELSE ISNULL(@commission, 0) + ISNULL(@referral_amount, 0)
      END
      
	-- Add the T-SQL statements to compute the return value here
      SET @perc = (@total_comm / ISNULL(@selling_price, 0))

	-- Return the result of the function
	RETURN @perc
END








