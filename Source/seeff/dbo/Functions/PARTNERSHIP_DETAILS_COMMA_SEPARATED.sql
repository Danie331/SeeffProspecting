


CREATE FUNCTION [dbo].[PARTNERSHIP_DETAILS_COMMA_SEPARATED] ( @partnership_id VARCHAR(50)) 
	RETURNS VARCHAR(255)
	AS BEGIN
		
		DECLARE @details VARCHAR(255)
		
		SELECT @details = COALESCE(@details + ', ', '') 
		+ a.agentFirstName 
		+ ' ' + a.agentSurname 
		+ ' (' 
		+  b.branchName 
		+ ', ' 
		+ CAST((SELECT COUNT(*) FROM [seeff].[dbo].[property] p WHERE p.fkAgentId = a.agentId AND propertyActive = 1) AS VARCHAR)
		+ ')'
		FROM agent a 
			INNER JOIN branch b ON(a.fkBranchId = branchId) 
		WHERE a.partnership_id = @partnership_id AND agentActive = 1 AND b.branchActive = 1
		GROUP BY a.agentId, agentFirstName, agentSurname, branchName
			
	RETURN SUBSTRING(@details, 2, LEN(@details))
	
END


/* SELECT [partnership_id], [dbo].[PARTNERSHIP_COMMA_SEPARATED_DETAILS](partnership_id) as 'content' FROM agent WHERE partnership_id != '' */


