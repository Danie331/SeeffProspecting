
-- =============================================
-- Author:		GW Swanepoel
-- Create date: 2012-06-07
-- Description:	Updates the total
--				column in table:
--				reports_license_summary
--				Add all values in columns
--				jan, feb, mar, apr, may, jun, jul,
--				etc together
-- =============================================
CREATE PROCEDURE [dbo].[spMIS_Update_TOTAL_reports_license_summary]
	-- Add the parameters for the stored procedure here
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	UPDATE reports_license_summary
	SET total = [jan]+[feb] +[mar] +[apr] +[may] +[jun] +[jul] +[aug] +[sep] +[oct] +[nov] +[dec]
END

