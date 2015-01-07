-- =============================================
-- Author:		Adam Roberts
-- Create date: 2014 02 10
-- Description:	Validate Moodle Users
-- =============================================
CREATE PROCEDURE [dbo].[moodle_validate_users]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

-- Suspend invalid Users
UPDATE [41.222.226.215].[moodle].[dbo].[mdl_user]
SET suspended = 1
WHERE username NOT IN (SELECT [user_email_address]
  FROM [41.222.226.213].[boss].[dbo].[user_registration]
WHERE [user_email_address] LIKE '%@seeff.com')
AND suspended = 0

-- Unsuspend valid users
UPDATE [41.222.226.215].[moodle].[dbo].[mdl_user]
SET suspended = 0
WHERE username IN (SELECT [user_email_address]
                     FROM [41.222.226.213].[boss].[dbo].[user_registration]
                    WHERE [user_email_address] LIKE '%@seeff.com')
AND suspended = 1

-- Unsuspend Ronel's second account
UPDATE [41.222.226.215].[moodle].[dbo].[mdl_user]
SET suspended = 0
WHERE username LIKE 'ronel.student@seeff.com'

END
