

-- =============================================
-- Author:		GW Swanepoel
-- Create date: 
-- Description:	Calculates the recognition AMOUNT
--				for Recognition reports
-- =============================================
CREATE FUNCTION [dbo].[fnMIS_SalesRecog_Solitaire_Amount] 
(
	-- Add the parameters for the function here
	@sps_transaction_ref VARCHAR(50)
	,@referral_type VARCHAR(50)
	,@registration_id INT
	,@comm_paid DECIMAL(18,2)
)
RETURNS DECIMAL(18,2)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @intConfirmedAgentCount INT
	DECLARE @intSPS_Agent_Split_AgentCount INT
	DECLARE @intDivider INT
	DECLARE @intResult DECIMAL(18,6)
	DECLARE @total_comm DECIMAL(18,6)
	DECLARE @shareComm DECIMAL(18,6)
	DECLARE @RecogAmount DECIMAL(18,6)

	-- Add the T-SQL statements to compute the return value here
	
		SET @intSPS_Agent_Split_AgentCount = (SELECT COUNT(*) FROM sps_agent_split INNER JOIN user_registration on sps_agent_split.registration_id = user_registration.registration_id  WHERE sps_agent_split.sps_transaction_ref = @sps_transaction_ref 
	AND recognise = 1 AND confirmation = 'Y')
	
	SET @total_comm =
	CASE @referral_type 
		WHEN 'External Paid to you' THEN (SELECT SUM(isnull(sps_comm_amount,0)) FROM sps_transaction WHERE sps_transaction.sps_transaction_ref = @sps_transaction_ref)
		WHEN 'Internal' THEN (SELECT SUM(isnull(sps_comm_amount,0) + isnull(sps_transaction.sps_referral_comm,0)) FROM sps_transaction WHERE sps_transaction.sps_transaction_ref = @sps_transaction_ref)
		ELSE
		  (SELECT SUM(isnull(sps_comm_amount,0)) FROM sps_transaction WHERE sps_transaction.sps_transaction_ref = @sps_transaction_ref)
	END 
	
	IF @intSPS_Agent_Split_AgentCount = 1
	BEGIN
		SET @RecogAmount = (@total_comm/0.07)
		RETURN @RecogAmount
	END
	
	IF @intSPS_Agent_Split_AgentCount > 1 
	BEGIN
	SET @RecogAmount = (@comm_paid / @total_comm) * (@total_comm/0.07)
	RETURN @RecogAmount
	END
			
RETURN @RecogAmount
END

