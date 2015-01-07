




-- =============================================
-- Author:		Adam Roberts
-- Create date: 2011/05/27
-- Description:	Calculates the sales commission to a 7 % value
-- =============================================
CREATE FUNCTION [dbo].[rental_comm_at_seven] 
(
	-- Add the parameters for the function here
	 @referral_type VARCHAR(100),
	 @referral_amount DECIMAL(18,2),
	 @commission DECIMAL(18,2),
	 @sps_rental_management_fee DECIMAL(18,2)
)
RETURNS DECIMAL(18,2)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @comm_at_seven DECIMAL(18,2);
    DECLARE @total_comm DECIMAL(18,2);
    
    SET @total_comm = 
      CASE @referral_type
         WHEN 'None' THEN ISNULL(@commission, 0) + ISNULL(@sps_rental_management_fee, 0)
         WHEN 'Inter-license' THEN ISNULL(@commission, 0) + ISNULL(@sps_rental_management_fee, 0)
         WHEN 'Internal' THEN ISNULL(@commission, 0) + ISNULL(@sps_rental_management_fee, 0) + ISNULL(@referral_amount, 0)
         WHEN 'External you paid' THEN ISNULL(@commission, 0) + ISNULL(@sps_rental_management_fee, 0)
         WHEN 'External paid to you' THEN ISNULL(@commission, 0) + ISNULL(@sps_rental_management_fee, 0)
         ELSE   ISNULL(@commission, 0) + ISNULL(@sps_rental_management_fee, 0)
      END
      
	-- Add the T-SQL statements to compute the return value here
      SET @comm_at_seven = (@total_comm / 0.07)

	-- Return the result of the function
	RETURN @comm_at_seven

END





