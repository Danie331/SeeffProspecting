

-- =============================================
-- Author:		Adam Roberts
-- Create date: 2011/05/27
-- Description:	Calculates the percentage that the sale was done at. 
-- =============================================
CREATE FUNCTION [dbo].[rental_comm_perc_managed] 
(
	-- Add the parameters for the function here
	 @sps_rental_management_fee DECIMAL(18,4),
	 @listing_price_monthly_rental DECIMAL(18,4)
)
RETURNS DECIMAL(18,4)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @perc DECIMAL(18,4);
    
    SET @perc = ISNULL(@sps_rental_management_fee, 0) / ISNULL(@listing_price_monthly_rental, 0); 
	RETURN @perc
END


