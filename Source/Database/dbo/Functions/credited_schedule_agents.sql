





-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[credited_schedule_agents] 
(
	-- Add the parameters for the function here
	@sps_transaction_ref VARCHAR(50)
)
RETURNS VARCHAR(MAX)
AS
BEGIN
	DECLARE @details VARCHAR(MAX) 
    SELECT @details = COALESCE(@details + ', ', '') + '(' + (SELECT CASE [registration_id] WHEN 1667 THEN 'NO AGENT' ELSE [user_preferred_name] + ' ' + [user_surname] END  
                                                             FROM [boss].[dbo].[user_registration]  
                                                             WHERE [boss].[dbo].[user_registration].[registration_id] = [boss].[dbo].[sps_agent_split].[registration_id]) + 
       CASE [boss].[dbo].[sps_agent_split].recognise
         WHEN 0 THEN ''
         WHEN 1 THEN ' - Top 20'
       END + ')' 
    FROM [boss].[dbo].[sps_agent_split]
    WHERE [sps_transaction_ref] LIKE @sps_transaction_ref
 
	RETURN @details 
END






