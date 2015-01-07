
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spMIS_Hub_Bank_Deals_Summary] 
	-- Add the parameters for the stored procedure here
	 @License_ID INT
	,@Create_BeginDate NVARCHAR(20)
	,@Create_EndDate NVARCHAR(20)
	,@License_Name NVARCHAR(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	/*Create temp table to hold all the values*/
CREATE TABLE #Hub_Deals_Per_Bank_License 
	(
		LicenseID INT
		,[License] NVARCHAR (100)
		,Bank_ProductID INT
		,Bank_Product NVARCHAR (100)
		,[No Mandates Received] INT
		,[No Mandates Accepted] INT
		,[No Mandates Declined] INT
		,[No Mandates Sold] INT
		,[Total Reserve Price] DECIMAL (18,2)
		,[Total Selling Price] DECIMAL (18,2)
		,[Total ESP Commission] DECIMAL (18,2)
	)
/*Create temp table to hold all the values*/

/*Insert all the deals allocated to the license
  for the particular period					 */	
	INSERT INTO #Hub_Deals_Per_Bank_License
	SELECT     license.license_id, license.license_name AS License, sps_hub_bankprod.product_id, sps_hub_bankprod.bank + ' ' + sps_hub_bankprod.product AS [Bank Product], 
                      COUNT(sps_tran.sps_created_date) AS [No Mandates Received], 0 AS Expr1, 0 AS Expr2, 0 AS Expr3, SUM(sps_hub_tran.reserve_price) AS [Reserve Price], 0 AS Expr4, 0 AS Expr5
FROM         sps_transaction AS sps_tran INNER JOIN
                      sps_hub_transaction AS sps_hub_tran ON sps_tran.sps_transaction_ref = sps_hub_tran.sps_transaction_ref INNER JOIN
                      sps_hub_bank_product AS sps_hub_bankprod ON sps_hub_tran.product_id = sps_hub_bankprod.product_id INNER JOIN
                      user_registration AS user_registration_1 ON sps_hub_tran.registration_id = user_registration_1.registration_id INNER JOIN
                      license_branches ON user_registration_1.branch_id = license_branches.branch_id INNER JOIN
                      license ON license_branches.license_id = license.license_id
WHERE     (license.license_id = @License_ID) AND (sps_tran.sps_created_date >= @Create_BeginDate) AND (sps_tran.sps_created_date <= @Create_EndDate) AND (sps_hub_tran.transaction_status NOT LIKE 'ARCHIVED')

GROUP BY
			  license.license_id
			, license.license_name
			, sps_hub_bankprod.bank + ' ' + sps_hub_bankprod.product
			, sps_hub_bankprod.product_id

  
/*Insert bank products that have no records*/ 			
INSERT INTO #Hub_Deals_Per_Bank_License
SELECT @License_ID
	   ,@License_Name
	   ,S.product_id
	   ,S.bank + ' ' + S.product AS [Bank Product]
	   ,0
	   ,0
	   ,0
	   ,0
	   ,0
	   ,0
	   ,0				
FROM sps_hub_bank_product S
WHERE S.product_id NOT IN (SELECT #Hub_Deals_Per_Bank_License.Bank_ProductID FROM #Hub_Deals_Per_Bank_License)		
/*Insert bank products that have no records*/

			
UPDATE #Hub_Deals_Per_Bank_License
	SET [No Mandates Accepted] = (
								  SELECT 
										COUNT(sps_tran.sps_created_date)
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
AND 
			( transaction_status LIKE 'ACCEPTED' )
			
AND #Hub_Deals_Per_Bank_License.Bank_ProductID = sps_hub_bankprod.product_id				
)

UPDATE #Hub_Deals_Per_Bank_License
	SET [No Mandates Declined] = (
								  SELECT 
										COUNT(sps_tran.sps_created_date)
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
AND 
			( transaction_status LIKE 'Declined' )
			
AND #Hub_Deals_Per_Bank_License.Bank_ProductID = sps_hub_bankprod.product_id				
)

UPDATE #Hub_Deals_Per_Bank_License
	SET [No Mandates Sold] = (
								  SELECT 
										COUNT(sps_tran.sps_created_date)
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
AND 
			( transaction_status LIKE 'Sold' )
			
AND #Hub_Deals_Per_Bank_License.Bank_ProductID = sps_hub_bankprod.product_id				
)

UPDATE #Hub_Deals_Per_Bank_License
	SET [Total Selling Price]  = (
								  SELECT 
										SUM(sps_tran.sps_selling_price)
								 FROM         
		   sps_transaction AS sps_tran 
INNER JOIN	
		   sps_hub_transaction AS sps_hub_tran ON sps_tran.sps_transaction_ref = sps_hub_tran.sps_transaction_ref 
INNER JOIN
		   sps_hub_bank_product AS sps_hub_bankprod ON sps_hub_tran.product_id = sps_hub_bankprod.product_id 
INNER JOIN
			user_registration AS user_registration_1 ON sps_hub_tran.registration_id = user_registration_1.registration_id 
INNER JOIN
			license_branches ON user_registration_1.branch_id = license_branches.branch_id 
INNER JOIN
			license ON license_branches.license_id = license.license_id
WHERE     
			(license.license_id = @License_ID) AND (sps_hub_tran.transaction_status LIKE 'Sold')
			
AND #Hub_Deals_Per_Bank_License.Bank_ProductID = sps_hub_bankprod.product_id				
)

UPDATE #Hub_Deals_Per_Bank_License
	SET [Total ESP Commission] = ([Total Selling Price] * 0.5)


/*Clean up: Remove bank products that have no deals:
  No allocated; Accepted; Declined and/or Sold*/
DELETE FROM #Hub_Deals_Per_Bank_License 
WHERE 
		([No Mandates Received] = 0) 
AND 
		([No Mandates Accepted] = 0) 
AND 
		([No Mandates Declined] = 0) 
AND 
		([No Mandates Sold] = 0)

SELECT * FROM #Hub_Deals_Per_Bank_License ORDER BY Bank_ProductID ASC

DROP TABLE #Hub_Deals_Per_Bank_License
END

