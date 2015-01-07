-- ======================================================
-- Author:		GW Swanepoel
-- Create date: 2013-09-24
-- Description:	calculates the referral recognition
--				earned for transactions:
--				Referral Type: 'External You Paid'/
--				Referral Type: 'External Paid to you'
--				Smart Pass must be present
--				Bonafide sale only
--				Agent not resigned (Confirmed = 'Y')
--				all of the above handled in the stored
--				procedure that calls this function
--				'External Paid to You' - 0.5
--				'External You Paid' - the difference: 0.5
-- =======================================================
CREATE FUNCTION [dbo].[fnMIS_ReferralRecog_Unit]
(
	-- Add the parameters for the function here
	@sps_transaction_ref VARCHAR(50)
	,@ReferralType VARCHAR(50)
	,@registration_id INT
	,@split_count INT
)
RETURNS DECIMAL(16,2)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @Result DECIMAL(16,2)
	DECLARE @intConfirmedAgentCount INT
	DECLARE @intConfirmedPartnershipCount INT
	DECLARE @intSPS_Agent_Split_AgentCount INT

	-- Add the T-SQL statements to compute the return value here

-- SELECT @intConfirmedPartnershipCount = (SELECT 
--		COUNT(DISTINCT(partnership_group.partnership_id))
-- FROM
--		partnership_group
-- WHERE
--		registration_id IN 
--		(
--		SELECT 
--				registration_id 
--		FROM 
--				sps_agent_split
--		WHERE
--				sps_agent_split.sps_transaction_ref = @sps_transaction_ref		
--		))

----Solitaires	
--SELECT @intConfirmedAgentCount = 
--(SELECT 
--		COUNT(registration_id) 
--FROM 
--		dbo.reports_Referral_Recognition_Detail 
--WHERE 
--		Agent_Count = 1	
--AND
--		sps_transaction_ref = @sps_transaction_ref		
--)
											
--SELECT @Result = (0.5 /(@intConfirmedAgentCount + @intConfirmedPartnershipCount ))

SELECT @Result = (0.5 / (SELECT COUNT(*) FROM dbo.reports_Referral_Recognition_Detail WHERE sps_transaction_ref = @sps_transaction_ref))

-- Return the result of the function
RETURN @Result

END
