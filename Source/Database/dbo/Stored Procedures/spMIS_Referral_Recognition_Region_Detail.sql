-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spMIS_Referral_Recognition_Region_Detail] 
	-- Add the parameters for the stored procedure here
	@region VARCHAR(50)
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here	
     
	SELECT * FROM dbo.reports_Referral_Recognition_Detail WHERE reports_Referral_Recognition_Detail.region = @region ORDER BY month_reported, transaction_division,[Agent_Count]
END
