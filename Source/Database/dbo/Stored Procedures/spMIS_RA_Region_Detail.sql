-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spMIS_RA_Region_Detail] 
	-- Add the parameters for the stored procedure here
	@Region NVARCHAR(50) = 'Northern'	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here	
     
	SELECT [Region]
      ,[License_ID]
      ,[License]
      ,[Agent]
      ,[Recognition Unit]
      ,[Recognition Amount]
      ,[SPS Seeff Referral]
      ,[SPS Referral Comm]
      ,[Comm Paid]
      ,[SPS Comm Amount]
      ,[SPS Percentage Split]
      ,[SPS Transaction Type]
      ,[SPS Transaction Division]
      ,[Reg ID]
      ,[Month]
      ,[Partner Count]
      ,[SPS Referral Type]
      ,[SPS Transaction ID]
      ,[SPS Transaction Ref]
      ,[SPS License Deal No]
      ,[SPS Sold Date]
      ,[SPS Selling Price]
      ,[SPS Property ID]
      ,[SPS Property Address]
      ,[SPS Created Date]
      ,[SPS Reporting Date]
      ,[Recognise]
      ,[Smart Pass ID]
      ,[Smart Pass License ID To]
      ,[Smart Pass License ID From]
      ,[InsertOn]
  FROM [boss].[dbo].[reports_Sales_Recognition_Detail_Global] 
  WHERE reports_Sales_Recognition_Detail_Global.Region = @Region 
  ORDER BY 
		Month
		,License
END
