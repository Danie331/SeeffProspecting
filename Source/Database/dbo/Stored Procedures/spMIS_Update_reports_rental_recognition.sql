


-- ====================================================
-- Author:		GW Swanepoel
-- Create date: 2012-06-13
-- Description:	Updates table
--				reports_rental_recognition
--				Field: units (based on solitaire/
--						partnership - how many members)
-- ====================================================
CREATE PROCEDURE [dbo].[spMIS_Update_reports_rental_recognition] 
	-- Add the parameters for the stored procedure here
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	UPDATE reports_rental_recognition
	SET units = 1 

	
END

