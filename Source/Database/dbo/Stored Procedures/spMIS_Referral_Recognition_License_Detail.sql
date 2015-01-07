-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spMIS_Referral_Recognition_License_Detail] 
	-- Add the parameters for the stored procedure here
	@LicID INT = 89
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here	
     
	SELECT * FROM dbo.reports_Referral_Recognition_Detail WHERE reports_Referral_Recognition_Detail.license_id = @LicID ORDER BY month_reported, [transaction_division],[Agent_Count]
END
