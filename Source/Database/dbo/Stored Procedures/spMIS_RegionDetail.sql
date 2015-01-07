-- =============================================
-- Author:		Gustav Swanepoel
-- Create date: 2012-02-09
-- Description:	Returns the DISTINCT Region
--				from the License table in BOSS
-- =============================================
CREATE PROCEDURE [dbo].[spMIS_RegionDetail]
	-- Add the parameters for the stored procedure here
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT DISTINCT Region FROM license WHERE region <> 'National'
END


