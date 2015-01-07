

-- =============================================
-- Author:		Adam Roberts
-- Create date: 2012/06/11
-- Description:	Return the last comment made on a referral
-- =============================================
CREATE FUNCTION [dbo].[last_smart_pass_comment] 
(
	@smart_pass_id int
)
RETURNS nvarchar(MAX) 
AS
BEGIN
	DECLARE @ResultVar nvarchar(MAX) 


    SELECT @ResultVar =  [subject] + ', ' + [comment]
      FROM [boss].[dbo].[smart_pass_comment]
     WHERE [smart_pass_comment_id] = (SELECT MAX(smart_pass_comment_id) AS smart_pass_comment_id
                                        FROM smart_pass_comment
                                       WHERE (smart_pass_id = @smart_pass_id))

	RETURN @ResultVar
END


