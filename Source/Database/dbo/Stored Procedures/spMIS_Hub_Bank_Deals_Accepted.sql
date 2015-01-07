
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spMIS_Hub_Bank_Deals_Accepted] 
	-- Add the parameters for the stored procedure here
	@License_ID INT
	--,@Create_BeginDate NVARCHAR(20) = '2012-12-01'
	--,@Create_EndDate NVARCHAR(20) = '2012-12-31'

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT     license.license_name AS [License]
			, CONVERT(VARCHAR(12),sps_tran.sps_created_date, 111) AS [Created Date]
			, sps_hub_tran.transaction_status AS [Transaction Status]
			, sps_tran.sps_lic_deal_no AS [License Deal Number]
			, sps_hub_bankprod.bank + ' ' + sps_hub_bankprod.product AS [Bank Product]
			, sps_tran.sps_property_address AS [Address]
			, sps_hub_tran.reserve_price AS [Reserve Price]
			, sps_tran.sps_selling_price AS [Selling Price]
FROM         
			sps_transaction AS sps_tran 
INNER JOIN
			sps_hub_transaction AS sps_hub_tran ON sps_tran.sps_transaction_ref = sps_hub_tran.sps_transaction_ref 
INNER JOIN
			sps_hub_bank_product AS sps_hub_bankprod ON sps_hub_tran.product_id = sps_hub_bankprod.product_id 
INNER JOIN
			sps_hub_comment ON sps_tran.sps_transaction_ref = sps_hub_comment.sps_transaction_ref 
INNER JOIN
			user_registration ON sps_hub_comment.registration_id = user_registration.registration_id 
INNER JOIN
			user_registration AS user_registration_1 ON sps_hub_tran.registration_id = user_registration_1.registration_id 
INNER JOIN
			license_branches ON user_registration_1.branch_id = license_branches.branch_id 
INNER JOIN
			license ON license_branches.license_id = license.license_id
WHERE 
			(
				sps_hub_comment.created_date = (
													SELECT
															(MAX(sps_hub_comment.created_date)) 
													FROM 
															sps_hub_comment 
													WHERE
															sps_tran.sps_transaction_ref = sps_hub_comment.sps_transaction_ref	
													GROUP BY 
															sps_hub_comment.sps_transaction_ref	
													)
			)
AND
			license.license_id = @License_ID
																															
AND ( transaction_status LIKE 'ACCEPTED' )		                      

END

