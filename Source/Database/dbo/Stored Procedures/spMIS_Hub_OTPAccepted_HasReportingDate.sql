
-- =============================================
-- Author:		Gustav Swanepoel
-- Create date: 2012-04-04
-- Description:	Returns transaction that have
--				been OTP Accepted AND has a 
--				reporting date in sps_transaction
--				For Mark's purposes.
--				(If the report returns no results
--				that is actually wanted)
-- =============================================
CREATE PROCEDURE [dbo].[spMIS_Hub_OTPAccepted_HasReportingDate] 
	-- Add the parameters for the stored procedure here

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	CREATE NONCLUSTERED INDEX Indx01 ON [dbo].[sps_transaction] ([sps_created_date])
		INCLUDE ([sps_transaction_ref],[sps_lic_deal_no],[sps_listing_price],[sps_reporting_date])

SELECT
						license.region AS [Region]
						,license.license_name AS [License]
						,ST.sps_lic_deal_no [License Deal No]
						,dbo.fnMIS_bank_product_name(SHP.bank,  SHP.product) AS [Bank Product]
						,CONVERT(VARCHAR(7), ST.sps_created_date, 120) AS YearMonth_Created
						,CONVERT(VARCHAR(7), ST.sps_reporting_date, 120) AS YearMonth_Reported
						,ST.sps_created_date AS [Created Date]
						,ST.sps_reporting_date AS [Reporting Date]
						,SHT.transaction_status AS Status
						,ISNULL(ST.sps_listing_price,0) AS [Listing Price]
						,ISNULL(SHT.invoice_number,' ') AS  [Invoice Number]
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
				 AND
						(SHT.transaction_status LIKE 'OTP ACCEPTED')						
					
					DROP INDEX Indx01 ON [dbo].[sps_transaction]
END

