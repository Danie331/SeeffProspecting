


-- =============================================
-- Author:		GW Swanepoel
-- Create date: 2012-07-12
-- Description:	Returns last comments for Hub
--				transactions (past 24 hours
--				and on Mondays: from Friday
--				17:00 till Monday 17:00)
-- =============================================
CREATE PROCEDURE [dbo].[spMIS_Hub_24Hour_Comments]
	-- Add the parameters for the stored procedure here
	@dtDateBegin NVARCHAR(30)
	,@dtDateEnd NVARCHAR(30)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	
	SELECT     sps_hub_bank_product.bank + ' ' + sps_hub_bank_product.product AS [Bank Product]
		   , sps_transaction.sps_web_ref AS [SPS Web Ref]
		   , sps_hub_comment.created_date AS [Created Date]
		   , sps_hub_transaction.transaction_status AS [Transaction Status]
		   , sps_hub_comment.comment AS [Comment]
		   , user_registration.user_name + ' ' + user_registration.user_surname AS [User]
		   , sps_transaction.sps_lic_deal_no AS [License Deal No]
	FROM         
				sps_transaction 
	INNER JOIN
				sps_hub_comment ON sps_transaction.sps_transaction_ref = sps_hub_comment.sps_transaction_ref 
	INNER JOIN
				user_registration ON sps_hub_comment.registration_id = user_registration.registration_id 
	INNER JOIN
				sps_hub_transaction ON sps_hub_comment.sps_transaction_ref = sps_hub_transaction.sps_transaction_ref 
	INNER JOIN
				sps_hub_bank_product ON sps_hub_transaction.product_id = sps_hub_bank_product.product_id
	WHERE     
				(sps_hub_comment.created_date BETWEEN @dtDateBegin AND @dtDateEnd)
	ORDER BY 
				[Bank Product], [Created Date] DESC
END



