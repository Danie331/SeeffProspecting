




-- =============================================
-- Author:		Gustav Swanepoel
-- Create date: 2012-03-28
-- Description:	Returns Agent name based on
--				sps_transaction.sps_transaction_ref
-- =============================================
CREATE FUNCTION [dbo].[fnMIS_AgentName] 
(
	-- Add the parameters for the function here
	@sps_transaction_ref VARCHAR(50)
)
RETURNS VARCHAR(MAX)
AS
BEGIN
	DECLARE @details VARCHAR(MAX) 
    SELECT @details = (SELECT CASE [registration_id] WHEN 1667 THEN 'NO AGENT' ELSE [user_preferred_name] + ' ' + [user_surname] END  
                                                               FROM [boss].[dbo].[user_registration]  
                                                              WHERE [boss].[dbo].[user_registration].[registration_id] = [boss].[dbo].[sps_agent_split].[registration_id])
    FROM [boss].[dbo].[sps_agent_split]
    WHERE [sps_transaction_ref] LIKE @sps_transaction_ref

	RETURN @details 
END
