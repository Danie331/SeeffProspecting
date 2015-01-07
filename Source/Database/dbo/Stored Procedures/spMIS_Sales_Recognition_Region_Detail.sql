
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spMIS_Sales_Recognition_Region_Detail] 
	-- Add the parameters for the stored procedure here
	@Region VARCHAR(50)
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here	
     
	SELECT * FROM dbo.reports_Sales_Recognition_Detail WHERE reports_Sales_Recognition_Detail.Region = @Region ORDER BY Month, [SPS Transaction Division],[Partner Count]
END

