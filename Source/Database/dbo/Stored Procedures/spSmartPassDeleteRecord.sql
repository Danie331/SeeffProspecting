-- =============================================
-- Author:		Adam Roberts
-- Create date: 15-03-2012
-- Description:	Completely remove a record from Smart Pass
-- =============================================
CREATE PROCEDURE [dbo].[spSmartPassDeleteRecord] 
	@smart_pass_id INT
AS
BEGIN
	SET NOCOUNT ON;
 IF @smart_pass_id > 0
   BEGIN
	DELETE FROM [boss].[dbo].[smart_pass]
	WHERE [smart_pass_id] = @smart_pass_id

	DELETE FROM [boss].[dbo].[smart_pass_actual]
	WHERE [smart_pass_id] = @smart_pass_id 

	DELETE  FROM [boss].[dbo].[smart_pass_comment]
	WHERE [smart_pass_id] = @smart_pass_id 

	DELETE FROM [boss].[dbo].[smart_pass_participants]
	WHERE [smart_pass_id] = @smart_pass_id 

	DELETE FROM [boss].[dbo].[smart_pass_transaction_follower]
	WHERE [smart_pass_id] = @smart_pass_id 
  END    
END
