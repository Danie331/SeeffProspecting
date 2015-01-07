

CREATE FUNCTION [dbo].[PARTNERSHIP_AGENTID_COMMA_SEPARATED] ( @partnership_id VARCHAR(50)) 
	
	RETURNS VARCHAR(100)
	AS 
	BEGIN
		DECLARE @agentId_string VARCHAR(100)
		SELECT @agentId_string = COALESCE(@agentId_string + ',', '') + CAST(agentId AS VARCHAR(5)) 
					FROM agent WHERE partnership_id = @partnership_id
	
		RETURN SUBSTRING(@agentId_string, 2, LEN(@agentId_string))
	
END


/*SELECT [partnership_id], [dbo].[partner_link](partnership_id) as 'content' FROM agent WHERE partnership_id != '' */

