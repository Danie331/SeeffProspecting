
-- =============================================
-- Author:		GW Swanepoel
-- Create date: 2012-05-15
-- Description:	Returns detail records
--				of Hub transactions received
--				Included Listing Price for
--				for monetary purposes
-- =============================================
CREATE PROCEDURE [dbo].[spMIS_Hub_Mandates_Received] 
	-- Add the parameters for the stored procedure here
		@DateBegin NVARCHAR(20)
	,@DateEnd NVARCHAR(20)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	 SELECT
						CONVERT(VARCHAR(7), ST.sps_created_date, 120) AS [Year Month Created]
						,license.region AS [Region]
						,license.license_name AS [License]
						,dbo.fnMIS_bank_product_name(SHP.bank,  SHP.product) AS [Bank Product]
						, SHT.valuation_price AS [Valuation Price]
						, SHT.reserve_price AS [Reserve Price]
						,CASE
							WHEN 
									SHT.valuation_price = 0 		
							THEN 
									SHT.reserve_price
							WHEN 
									SHT.reserve_price = 0 
							THEN 
									SHT.valuation_price						 
							WHEN 
									(ISNULL(SHT.valuation_price,SHT.RESERVE_PRICE)) <= (ISNULL(SHT.reserve_price,SHT.valuation_price)) 
							THEN
									(ISNULL(SHT.valuation_price,SHT.reserve_price))
							WHEN
									(ISNULL(sht.reserve_price,SHT.valuation_price)) <= (ISNULL(SHT.valuation_price,SHT.reserve_price)) 
							THEN
									(ISNULL(SHT.reserve_price,SHT.valuation_price))
							ELSE
									NULL	
						 END AS [Valuation/Reserve Price]									
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
						(ST.sps_created_date >= @DateBegin)
                 AND
						(ST.sps_created_date <= @DateEnd)
						
END



