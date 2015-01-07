
-- =============================================
-- Author:		GW Swanepoel
-- Create date: 2012-04-24
-- Description:	Returns Detail records
--				of Hub records that are:
--				Reported in sps_transaction
--				Status = Sold
--				Payment Date IS NULL
-- =============================================
CREATE PROCEDURE [dbo].[spMIS_Hub_Pipeline_with_ActSellPrice] 
	-- Add the parameters for the stored procedure here
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT
						license.region AS [Region]
						,license.license_name AS [License]
						,ST.sps_lic_deal_no [License Deal No]
						,dbo.fnMIS_bank_product_name(SHP.bank,  SHP.product) AS [Bank Product]
						,CONVERT(VARCHAR(7), ST.sps_created_date, 120) AS YearMonth_Created
						,CONVERT(VARCHAR(7), ST.sps_reporting_date, 120) AS YearMonth_Reported
						,ST.sps_created_date AS [Created Date]
						,ST.sps_sold_date AS [Sold Date]
						,ST.sps_reporting_date AS [Reporting Date]
						,SHT.transaction_status AS [Status]
						,ST.sps_listing_price AS [Listing Price]
						--, ST.sps_percentage_split --can be removed once testing is complete
						--,ST.sps_refferal_type AS [Referral Type] --can be removed once testing is complete
						--,ST.sps_referral_comm AS [Referral Comm] --can be removed once testing is complete
						,ST.sps_comm_amount AS [Your Comm Amount] 
						,ST.sps_selling_price AS [Selling Price]
						,dbo.sales_actual_selling(ST.sps_refferal_type,ISNULL(ST.sps_referral_comm,0),ISNULL(ST.sps_comm_amount,0),ISNULL(ST.sps_selling_price,0)) AS [Actual Selling Price]
						,SHT.invoice_number AS  [Invoice Number]
						,SHT.payment_date AS [Payment Date]
						, ISNULL(dbo.person_by_roll(ST.sps_transaction_ref, 'seller'),'') AS [Seller]
						, ISNULL(dbo.person_by_roll(ST.sps_transaction_ref, 'buyer'),'') AS [Buyer]
						
				 FROM         
						sps_hub_transaction AS SHT 						
				 INNER JOIN
                        sps_hub_bank_product AS SHP ON SHT.product_id = SHP.product_id 
                 INNER JOIN
                      sps_transaction AS ST ON SHT.sps_transaction_ref = ST.sps_transaction_ref 
                 INNER JOIN
                      user_registration ON SHT.registration_id = user_registration.registration_id 
                 INNER JOIN
                      license_branches ON user_registration.branch_id = license_branches.branch_id 
                 INNER JOIN
                      license ON license_branches.license_id = license.license_id
                 WHERE 
						(ST.sps_reporting_date IS NOT NULL)
--				 AND
--						(SHT.transaction_status LIKE 'SOLD')
				 AND
						 SHT.payment_date IS NULL											
				ORDER BY 
						YearMonth_Reported DESC	

END

