-- =============================================
-- Details : Unsubscribes client from mail
-- Author: Scott Murray
-- =============================================
CREATE PROCEDURE [dbo].[mymail_unsubscribe_client]
	@emailAddress varchar(250)
AS
BEGIN
  UPDATE [41.222.226.215].[my_mail].[dbo].[client]
     SET [blocked] = 1
	    ,[blocked_date] = GETDATE()
		,[active] = 0
   WHERE [email] = @emailAddress
END
