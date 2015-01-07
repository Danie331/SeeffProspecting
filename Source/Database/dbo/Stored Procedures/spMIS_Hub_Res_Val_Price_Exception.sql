
-- =============================================
-- Author:		Gustav Swanepoel
-- Create date: 2012/04/19
-- Description:	Returns HUB records
--				with no Reserve or Valuation 
--				Price
--				Exception Report
-- =============================================
CREATE PROCEDURE [dbo].[spMIS_Hub_Res_Val_Price_Exception]
	-- Add the parameters for the stored procedure here
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT    
		    license.region AS [Region]
		  , UR.user_preferred_name + ' ' + UR.user_surname AS [Hub Representative]
		  , license.license_name AS [License]
		  , user_registration.user_preferred_name + ' ' + user_registration.user_surname AS [License Representative]
		  , ST.sps_lic_deal_no AS [Deal No.]
		  --, ST.sps_listing_price AS [Listing Price]
		  , SHT.reserve_price AS [Reserve Price]
		  , SHT.valuation_price AS [Valuation Price]
		  , SHT.transaction_status AS [Trans Status]
		  , ST.sps_created_date  AS [Date Created]
		  , ST.sps_sold_date AS [Sold Date]
		  , ST.sps_selling_price AS [Selling Price]
		  , ST.sps_comm_amount AS [Comm Amount]
FROM         
		  sps_hub_transaction AS SHT
INNER JOIN
          user_registration ON SHT.registration_id = user_registration.registration_id 
INNER JOIN
		  license_branches ON user_registration.branch_id = license_branches.branch_id 
INNER JOIN
		  license ON license_branches.license_id = license.license_id 
INNER JOIN
		  sps_transaction AS ST ON SHT.sps_transaction_ref = ST.sps_transaction_ref 
INNER JOIN
		 user_registration AS UR ON ST.created_by = UR.registration_id	 
WHERE
		  ST.sps_created_date >= '2011/11/01 00:00:000'
AND
		  (SHT.reserve_price IS NULL)
AND
		  (SHT.valuation_price IS NULL)			  	  
ORDER BY
		  SHT.transaction_status 
		  , sps_created_date ASC
END

