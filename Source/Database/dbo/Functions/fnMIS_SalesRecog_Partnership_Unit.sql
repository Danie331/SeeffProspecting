-- =============================================
-- Author:		GW Swanepoel
-- Create date: 2013-08-18
-- Description:	Calculates the recognition
--				for Recognition reports
-- =============================================
CREATE FUNCTION [dbo].[fnMIS_SalesRecog_Partnership_Unit] 
(
	-- Add the parameters for the function here
	@sps_transaction_ref VARCHAR(50)
	,@referral_type VARCHAR(50) 
	,@registration_id INT
	,@split_count INT
	,@comm_paid DECIMAL(18,6)
	
)
RETURNS DECIMAL(18,6)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @intConfirmedAgentCount INT
	DECLARE @intSPS_Agent_Split_AgentCount INT
	DECLARE @intDivider INT
	DECLARE @intResult DECIMAL(18,6)
	DECLARE @total_comm DECIMAL(18,6)
	DECLARE @shareComm DECIMAL(18,6)
	DECLARE @RecogUnit DECIMAL(18,6)

	-- Add the T-SQL statements to compute the return value here
	SET @total_comm = (SELECT 0)	

	SET @intConfirmedAgentCount = (SELECT COUNT(*) FROM user_registration WHERE user_registration.registration_id in (SELECT sps_agent_split.registration_id from SPS_AGENT_SPLIT where sps_transaction_ref = @sps_transaction_ref) and user_registration.confirmation = 'Y')

SET @intSPS_Agent_Split_AgentCount = (SELECT COUNT(*) FROM sps_agent_split WHERE sps_agent_split.sps_transaction_ref = @sps_transaction_ref)

IF @intConfirmedAgentCount = 1 
	BEGIN
			SET @comm_paid = (SELECT SUM(sps_agent_split.comm_paid) FROM sps_agent_split WHERE sps_agent_split.sps_transaction_ref = @sps_transaction_ref)
	END
	
if @intConfirmedAgentCount > 1 and 
	@intConfirmedAgentCount < @intSPS_Agent_Split_AgentCount
	BEGIN
	--SET @intDivider = (@intSPS_Agent_Split_AgentCount - @intConfirmedAgentCount)

	--SET @shareComm = (SELECT SUM(sps_agent_split.comm_paid) FROM sps_agent_split WHERE sps_agent_split.registration_id IN (SELECT user_registration.registration_id FROM user_registration where confirmation = 'N') AND sps_agent_split.sps_transaction_ref = @sps_transaction_ref) / @intDivider
	
SET @total_comm = 	(SELECT SUM(sps_agent_split.comm_paid) FROM sps_agent_split WHERE sps_agent_split.registration_id IN (SELECT user_registration.registration_id FROM user_registration where confirmation = 'Y') AND sps_agent_split.sps_transaction_ref = @sps_transaction_ref)
--set @comm_paid = 	(@comm_paid + @shareComm)
	
END	
	
	IF @referral_type = 'External paid to you'
		BEGIN
	 		SET @RecogUnit =  (SELECT 
										((sps_transaction.sps_percentage_split/100) / @split_count)
							   FROM 
										sps_transaction 
							   WHERE 
										sps_transaction.sps_transaction_ref = @sps_transaction_ref
							   )
			RETURN @RecogUnit				   
		END					   
	 		
		IF @total_comm = 0 
		BEGIN
		SET @total_comm = (SELECT 
									ISNULL(SUM(sps_agent_split.comm_paid),0) 
						   FROM 
									sps_agent_split 
						   WHERE 
									sps_transaction_ref = @sps_transaction_ref 	
							)
		
		
		END
		
		IF @total_comm = 0 
		BEGIN
			RETURN 0
		END	
		ELSE
			BEGIN
			SET @RecogUnit = @comm_paid/@total_comm		
			
			END	
			RETURN @RecogUnit		
END
