-- =============================================
-- Author:		Adam Roberts
-- Create date: 2014-02-27
-- Description:	Returm the full name of the moodle course
-- =============================================
CREATE PROCEDURE moodle_course_name
	-- Add the parameters for the stored procedure here
	@course_id int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT fullname
      FROM [41.222.226.215].[moodle].[dbo].[mdl_course]
     WHERE id = @course_id
END
