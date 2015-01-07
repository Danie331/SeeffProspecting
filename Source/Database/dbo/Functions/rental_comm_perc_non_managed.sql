









-- =============================================
-- Author:		Adam Roberts
-- Create date: 2011/05/27
-- Description:	Calculates the comm percentage on a non managed lease. 
-- =============================================
CREATE FUNCTION [dbo].[rental_comm_perc_non_managed] 
(
	-- Add the parameters for the function here
	 @referral_amount DECIMAL(18,4),
	 @commission DECIMAL(18,4),
	 @listing_price_monthly_rental DECIMAL(18,4),
	 @start_date datetime,
	 @end_date datetime
)
RETURNS DECIMAL(18,4)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @total_comm DECIMAL(18,4);
    DECLARE @rental_days INT;
    DECLARE @daily_comm DECIMAL(18,4);
    DECLARE @daily_rental DECIMAL(18,4);
    DECLARE @perc DECIMAL(18,4);
    
    SET @total_comm = ISNULL(@commission, 0)  + ISNULL(@referral_amount, 0);
    SET @rental_days = DateDiff(DAY, @start_date, @end_date); 
    SET @daily_comm = @total_comm / (@rental_days + 1);
    SET @daily_rental = (ISNULL(@listing_price_monthly_rental, 0) * 12) / 365
    SET @perc = @daily_comm / @daily_rental  
    
    RETURN @perc
END










