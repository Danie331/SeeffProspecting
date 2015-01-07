

-- =============================================
-- Author:		Gustav Swanepoel
-- Create date: 2012-03-30
-- Description:	Based on query provided by Adam
--				used to extract the SOLD data
--				from the Hub transactions for
--				Mark Jaftha
--				I've added a month year report
--				column and changed the sorting
--				to sps_reporting_date DESC
-- =============================================
CREATE PROCEDURE [dbo].[spMIS_HUB_Sold_Report]
	-- Add the parameters for the stored procedure here
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT     
			CONVERT(VARCHAR(7),sps_transaction.sps_reporting_date,120) AS [Month Year Reported]
		   ,sps_transaction.sps_lic_deal_no AS [License Deal No]
		   ,sps_hub_transaction.transaction_status AS [Transaction Status]
		   ,sps_transaction.sps_reporting_date AS [Reporting Date]
		   ,(user_registration.user_preferred_name + ' ' + user_registration.user_surname) AS [Update By]
FROM         
			sps_transaction 
INNER JOIN
			sps_hub_transaction ON sps_transaction.sps_transaction_ref = sps_hub_transaction.sps_transaction_ref 
INNER JOIN
			user_registration ON sps_transaction.update_by = user_registration.registration_id
WHERE     
			(sps_transaction.branch_id IN (260, 263, 277))
AND 
			sps_hub_transaction.transaction_status LIKE 'Sold'
ORDER BY 
			sps_transaction.sps_reporting_date DESC
END


