
-- =============================================
-- Author:		Adam Roberts
-- Create date: 2014-02-24
-- Description:	Call Moodle to get complete user courses
-- =============================================
CREATE PROCEDURE [dbo].[moodle_user_courses] 
	-- Add the parameters for the stored procedure here
	@user_email_address varchar(512)
AS
BEGIN
	SET NOCOUNT ON;

    EXECUTE [41.222.226.215].moodle.[dbo].[completed_courses]  @user_email_address

END



