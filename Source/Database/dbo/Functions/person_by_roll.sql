





-- =============================================
-- Author:		Adam Roberts
-- Create date: 2011/06/03
-- Description:	Return all the people party to a transaction by roll
-- =============================================
CREATE FUNCTION [dbo].[person_by_roll] 
(
	-- Add the parameters for the function here
	@sps_transaction_ref VARCHAR(50),
	@roll VARCHAR(50)
)
RETURNS VARCHAR(MAX)
AS
BEGIN
	DECLARE @details VARCHAR(MAX) 
    SELECT @details = COALESCE(@details + ', ', '') + (SELECT sps_title + ' ' + sps_name + ' ' + sps_surname  
                                                               FROM [boss].[dbo].[sps_person]  
                                                              WHERE [boss].[dbo].[sps_person].[sps_person_id] = [boss].[dbo].[persons_in_transaction].[person_id])
      FROM [boss].[dbo].[persons_in_transaction]
    WHERE [transaction_guid] LIKE @sps_transaction_ref
      AND [person_roll] LIKE @roll
 	RETURN @details 
END






