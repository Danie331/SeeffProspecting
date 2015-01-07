







-- =============================================
-- Author:		Adam Roberts
-- Create date: 2011/05/27
-- Description:	Calculates the percentage that the sale was done at. 
-- =============================================
CREATE FUNCTION [dbo].[rental_remaining_period] 
(
	-- Add the parameters for the function here
	 @end_date datetime
)
RETURNS VARCHAR(100)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @months_left INT;
    DECLARE @days_left INT;
    DECLARE @message VARCHAR(100);
          
	-- Add the T-SQL statements to compute the return value here
	-- DATEDIFF(MONTH, GETDATE(), sps_transaction.sps_sold_date),
    SET @months_left = DATEDIFF(MONTH, GETDATE(), @end_date) 
    SET @days_left = DATEDIFF(DAY, GETDATE(), @end_date) 
    
    IF @days_left > 30
      SET @message = CONVERT(VARCHAR, @months_left) + ' Months'
    ELSE
      SET @message = CONVERT(VARCHAR, @days_left) + ' Days';
    
    SET @message = 
      CASE 
         WHEN @days_left = 0 THEN 'Expires Today'
         WHEN @days_left < 0 THEN 'Expired'
         ELSE @message
      END
	-- Return the result of the function
	RETURN @message
END








