
-- =============================================
-- Author:		Gustav Swanepoel
-- Create date: 2012-03-28
-- Description:	Returns Agent Registration ID
--				sps_transaction.sps_transaction_ref
-- =============================================
create FUNCTION [dbo].[fnMIS_AgentRegID]
(
	-- Add the parameters for the function here
	@sps_transaction_ref VARCHAR(50)
)
RETURNS INT
AS
BEGIN
	DECLARE @details INT 
    SELECT @details = (
					   SELECT 
								[registration_id]   
					   FROM 
								[boss].[dbo].[user_registration]  
					   WHERE 
								[boss].[dbo].[user_registration].[registration_id] = [boss].[dbo].[sps_agent_split].[registration_id]
					   )
	FROM 
			[boss].[dbo].[sps_agent_split]
    WHERE 
			[sps_transaction_ref] LIKE @sps_transaction_ref

	RETURN @details 
END