-- =============================================
-- Author:		GW Swanepoel
-- Create date: 2012-07-16
-- Description:	Returns detail regarding
--				Hub transactions
--				per transaction status
-- =============================================
CREATE PROCEDURE [dbo].[spMIS_HLR_PerLicense] 
	-- Add the parameters for the stored procedure here
	@Status NVARCHAR(100)
	,@LicenseID Integer
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	CREATE NONCLUSTERED INDEX Indx01
ON [dbo].[sps_transaction] ([sps_transaction_ref])
INCLUDE ([sps_lic_deal_no],[sps_solemandate],[sps_property_address],[sps_created_date])
	
IF @Status <> 'ALL' 

BEGIN
    -- Insert statements for procedure here
    
	SELECT     license.license_name AS [License]
		   , sps_tran.sps_solemandate AS [SPS Sole Mandate]
		   , CASE 
				WHEN reversed_deal = 0 THEN 'No' 
				WHEN reversed_deal = 1 THEN 'Yes' 
				ELSE '' 
				END AS [Reversed Deal]
			, CONVERT(VARCHAR(12),sps_tran.sps_created_date, 111) AS [Created Date]
			, sps_hub_tran.mandate_length AS [Mandate Length]
			, CONVERT(VARCHAR(12), DATEADD(DAY,sps_hub_tran.mandate_length, sps_tran.sps_created_date), 111) AS [Expires On]
			, CASE 
				WHEN sps_tran.sps_solemandate LIKE 'YES' 
				THEN DATEDIFF(DAY, GETDATE(), (DATEADD(DAY, sps_hub_tran.mandate_length, 
                      sps_tran.sps_created_date)))  
			  ELSE '' 
			  END AS [Sole Mandate Expires in]
			, DATEDIFF(DAY, sps_tran.sps_created_date, GETDATE()) AS [Number Days Mandate Received]
			, sps_hub_tran.transaction_status AS [Transaction Status]
			, sps_tran.sps_lic_deal_no AS [License Deal Number]
			, sps_hub_bankprod.bank + ' ' + sps_hub_bankprod.product AS [Bank Product]
			, sps_tran.sps_property_address AS [Address]
			, CONVERT(VARCHAR(12),sps_hub_comment.created_date, 111) AS [Created Date]
			, CASE
							WHEN 
									sps_hub_tran.valuation_price = 0 		
							THEN 
									sps_hub_tran.reserve_price
							WHEN 
									sps_hub_tran.reserve_price = 0 
							THEN 
									sps_hub_tran.valuation_price						 
							WHEN 
									(ISNULL(sps_hub_tran.valuation_price,sps_hub_tran.RESERVE_PRICE)) <= (ISNULL(sps_hub_tran.reserve_price,sps_hub_tran.valuation_price)) 
							THEN
									(ISNULL(sps_hub_tran.valuation_price,sps_hub_tran.reserve_price))
							WHEN
									(ISNULL(sps_hub_tran.reserve_price,sps_hub_tran.valuation_price)) <= (ISNULL(sps_hub_tran.valuation_price,sps_hub_tran.reserve_price)) 
							THEN
									(ISNULL(sps_hub_tran.reserve_price,sps_hub_tran.valuation_price))
							ELSE
									NULL	
						 END AS [Valuation/Reserve Price]
			, sps_hub_comment.comment AS Comment
			, user_registration.user_preferred_name + ' ' + user_registration.user_surname AS [User]
						, CASE 
				WHEN sps_tran.sps_solemandate LIKE 'YES' THEN 1
				WHEN sps_tran.sps_solemandate LIKE 'NO' THEN 0
				ELSE '99'
				END AS [SMCount]
			, CAST(reversed_deal AS INTEGER) AS [RDCount]
			, sps_tran.sps_reg_date AS [Registered]
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
AND ( sps_hub_tran.transaction_status = @Status)

AND (license.license_id = @LicenseID)

AND (transaction_status NOT LIKE 'ARCHIVED')
																
ORDER BY reversed_deal DESC			                      
END

IF @Status = 'ALL'

BEGIN
    -- Insert statements for procedure here
	SELECT     license.license_name AS [License]
		   , sps_tran.sps_solemandate AS [SPS Sole Mandate]
		   , CASE 
				WHEN reversed_deal = 0 THEN 'No' 
				WHEN reversed_deal = 1 THEN 'Yes' 
				ELSE '' 
				END AS [Reversed Deal]
			, CONVERT(VARCHAR(12),sps_tran.sps_created_date, 111) AS [Created Date]
			, sps_hub_tran.mandate_length AS [Mandate Length]
			, CONVERT(VARCHAR(12), DATEADD(DAY,sps_hub_tran.mandate_length, sps_tran.sps_created_date), 111) AS [Expires On]
			, CASE 
				WHEN sps_tran.sps_solemandate LIKE 'YES' 
				THEN DATEDIFF(DAY, GETDATE(), (DATEADD(DAY, sps_hub_tran.mandate_length, 
                      sps_tran.sps_created_date)))  
			  ELSE '' 
			  END AS [Sole Mandate Expires in]
			, DATEDIFF(DAY, sps_tran.sps_created_date, GETDATE()) AS [Number Days Mandate Received]
			, sps_hub_tran.transaction_status AS [Transaction Status]
			, sps_tran.sps_lic_deal_no AS [License Deal Number]
			, sps_hub_bankprod.bank + ' ' + sps_hub_bankprod.product AS [Bank Product]
			, sps_tran.sps_property_address AS [Address]
			, CONVERT(VARCHAR(12),sps_hub_comment.created_date, 111) AS [Created Date]
			, CASE
							WHEN 
									sps_hub_tran.valuation_price = 0 		
							THEN 
									sps_hub_tran.reserve_price
							WHEN 
									sps_hub_tran.reserve_price = 0 
							THEN 
									sps_hub_tran.valuation_price						 
							WHEN 
									(ISNULL(sps_hub_tran.valuation_price,sps_hub_tran.RESERVE_PRICE)) <= (ISNULL(sps_hub_tran.reserve_price,sps_hub_tran.valuation_price)) 
							THEN
									(ISNULL(sps_hub_tran.valuation_price,sps_hub_tran.reserve_price))
							WHEN
									(ISNULL(sps_hub_tran.reserve_price,sps_hub_tran.valuation_price)) <= (ISNULL(sps_hub_tran.valuation_price,sps_hub_tran.reserve_price)) 
							THEN
									(ISNULL(sps_hub_tran.reserve_price,sps_hub_tran.valuation_price))
							ELSE
									NULL	
						 END AS [Valuation/Reserve Price]
			, sps_hub_comment.comment AS Comment
			, user_registration.user_preferred_name + ' ' + user_registration.user_surname AS [User]
			, CASE 
				WHEN sps_tran.sps_solemandate LIKE 'YES' THEN 1
				WHEN sps_tran.sps_solemandate LIKE 'NO' THEN 0
				ELSE '99'
				END AS [SMCount]
			, CAST(reversed_deal AS INTEGER) AS [RDCount]
			,sps_tran.sps_reg_date AS [Registered]	
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
													
AND ( transaction_status NOT LIKE 'ARCHIVED' )

AND (license.license_id = @LicenseID)

			
ORDER BY reversed_deal DESC			                      

END

DROP INDEX Indx01 ON [dbo].[sps_transaction]
		
END







