
-- =============================================
-- Author:		GW Swanepoel
-- Create date: 
-- Description:	Calculates the recognition AMOUNT
--				for Recognition reports
-- =============================================
CREATE FUNCTION [dbo].[fnMIS_SalesRecog_Partnership_Amount] 
(
	-- Add the parameters for the function here
	@sps_transaction_ref VARCHAR(50)
	,@referral_type VARCHAR(50)
	,@registration_id INT
	,@split_count INT
	,@comm_paid DECIMAL(18,2)
)
RETURNS DECIMAL(18,2)
AS
BEGIN
	-- Declare the return variable here
DECLARE @intAgentRecog INT --this will be how many agents in the partnership has been marked for recognition
DECLARE @RecogAmount DECIMAL(18,2) -- this is the amount that will be recognized for
DECLARE @TotalComm DECIMAL(18,2) --this is the total commission to work with when calculation recognition amount
								 --exception: where intAgentRecog is = 1
								 
	-- Add the T-SQL statements to compute the return value here
	
	SET @TotalComm = (SELECT 0)

	--is the agent still a confirmed agent AND is the agent marked as recognize - if not in either criteria then won't be counted
	SET @intAgentRecog = (SELECT DISTINCT COUNT(*) FROM user_registration WHERE user_registration.registration_id 
						  IN (SELECT sps_agent_split.registration_id from SPS_AGENT_SPLIT where sps_transaction_ref = @sps_transaction_ref 
							  AND sps_agent_split.recognise = 1
							  ) 
						  AND 
							user_registration.confirmation = 'Y')

--here we go...
--this deals specifically where we only have only confirmed and recognized agent
IF @intAgentRecog = 0 
BEGIN
	set @RecogAmount = 0
END

IF @intAgentRecog = 1 
	BEGIN
			--now we have to test for what kind of referral_type we are working with
			IF @referral_type <> 'Internal' --it is only for referral_type = 'Internal' where sps_referral_comm  comes into play
				BEGIN
				
				SET @RecogAmount = (SELECT ((SUM(sps_comm_amount))/0.07) FROM sps_transaction WHERE sps_transaction.sps_transaction_ref = @sps_transaction_ref)
				
				--we don't have to drag this thing out any longer: return the recognition amount and be done with the function! (and save time)
				RETURN @RecogAmount				
				
				END
	END
	IF @referral_type = 'Internal' --referral type is internal so we have to add the referral_comm to the calculation
	BEGIN		
	
	SET @RecogAmount = (SELECT ((SUM(sps_comm_amount + sps_transaction.sps_referral_comm))/0.07) FROM sps_transaction WHERE sps_transaction.sps_transaction_ref = @sps_transaction_ref)
			--same thing here as in the previous if statement: we have the 
			RETURN @RecogAmount
	END

IF @intAgentRecog > 1 	
			BEGIN
					SET @TotalComm = (SELECT ISNULL(SUM(sps_agent_split.comm_paid),0) FROM sps_agent_split WHERE sps_transaction_ref = @sps_transaction_ref and
										sps_agent_split.registration_id IN (SELECT user_registration.registration_id FROM user_registration WHERE user_registration.confirmation = 'Y')	 AND sps_agent_split.recognise = 1)
					
					IF @referral_type <> 'Internal'
					BEGIN
					SET @TotalComm = (SELECT 
										ISNULL(SUM(sps_agent_split.comm_paid),0) 
									  FROM 
											sps_agent_split 
									   WHERE 
											(sps_transaction_ref = @sps_transaction_ref)
									   AND
											(recognise = 1)	 	
									  )  
					
					SET @RecogAmount =  (SELECT DISTINCT
								
									(ISNULL(sps_comm_amount,0))/0.07 
							  FROM 
									sps_transaction
							  INNER JOIN
									sps_agent_split ON sps_transaction.sps_transaction_ref = sps_agent_split.sps_transaction_ref		 
							  WHERE 
									(sps_transaction.sps_transaction_ref = @sps_transaction_ref)
							  AND
									sps_agent_split.recognise = 1
							  AND
									sps_agent_split.registration_id = @registration_id		
									)  * (@comm_paid/@TotalComm)				
					END
					ELSE
					BEGIN
					SET @TotalComm = (SELECT 
										ISNULL(SUM(sps_agent_split.comm_paid),0) 
									  FROM 
											sps_agent_split 
									   WHERE 
											(sps_transaction_ref = @sps_transaction_ref)
									   AND
											(recognise = 1)	 	
									  )  
					
					SET @RecogAmount =  (SELECT
								
									(ISNULL(sps_comm_amount,0) + ISNULL(sps_referral_comm,0))/0.07 
							  FROM 
									sps_transaction
							  INNER JOIN
									sps_agent_split ON sps_transaction.sps_transaction_ref = sps_agent_split.sps_transaction_ref		 
							  WHERE 
									(sps_transaction.sps_transaction_ref = @sps_transaction_ref)
							  AND
									sps_agent_split.recognise = 1
							  AND
									sps_agent_split.registration_id = @registration_id		
									)  * (@comm_paid/@TotalComm)
					
					RETURN @RecogAmount
				END				
END
RETURN @RecogAmount
END