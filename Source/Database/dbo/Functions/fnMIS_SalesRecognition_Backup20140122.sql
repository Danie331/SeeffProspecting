

-- =============================================
-- Author:		GW Swanepoel
-- Create date: 
-- Description:	Calculates the recognition AMOUNT
--				for Recognition reports
-- =============================================
create FUNCTION [dbo].[fnMIS_SalesRecognition_Backup20140122] 
(
	-- Add the parameters for the function here
	@sps_transaction_ref VARCHAR(50)
	,@registration_id INT
	,@comm_paid DECIMAL(18,6)
	,@SaleAtSeven DECIMAL(18,6)
	,@IsUnit BIT
	,@ReferralType VARCHAR(50)
)
RETURNS DECIMAL(18,6)
AS
BEGIN
	-- Declare the return variable here
DECLARE @RecogAmount DECIMAL(18,6)
DECLARE @RecogUnit DECIMAL(18,6)
DECLARE @ActualPaid DECIMAL(18,6)
DECLARE @Result DECIMAL(18,6)

	-- Add the T-SQL statements to compute the return value here
	SET @ActualPaid = (SELECT SUM(comm_paid) 
                         FROM sps_agent_split 
                        WHERE recognise = 1 
                          AND sps_agent_split.sps_transaction_ref = @sps_transaction_ref 
                          AND registration_id <> 1667)
	
	SET @RecogUnit = 	(@comm_paid / @ActualPaid)
	
	SET @RecogAmount = (@RecogUnit * @SaleAtSeven) 
	
IF @ReferralType = 'External paid to you'
BEGIN
DECLARE @split_count INT

SET @split_count = (SELECT COUNT(*) FROM sps_agent_split 
					WHERE sps_agent_split.sps_transaction_ref = @sps_transaction_ref
					AND registration_id <> 1667
					AND recognise = 1) 

SET @RecogUnit =  (SELECT 
										((sps_transaction.sps_percentage_split/100) / @split_count)
							   FROM 
										sps_transaction 
							   WHERE 
										sps_transaction.sps_transaction_ref = @sps_transaction_ref
							   )
			--RETURN @RecogUnit			
END	
		
SET @Result =
	CASE @IsUnit
	WHEN 1 THEN   @RecogUnit
	WHEN 0 THEN   @RecogAmount
ELSE
	@RecogAmount
END

RETURN @Result

END