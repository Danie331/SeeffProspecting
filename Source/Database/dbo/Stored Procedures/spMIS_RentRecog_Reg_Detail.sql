

-- ================================================
-- Author:		GW Swanepoel
-- Create date: 2012-06-18
-- Description:	Returns detail from table:
--				reports_rental_recognition
--				Used in: Rental Recognition report
-- ================================================
CREATE PROCEDURE [dbo].[spMIS_RentRecog_Reg_Detail]
	-- Add the parameters for the stored procedure here
	 @strRegion NVARCHAR(20)
	,@Begin_Date DATETIME
	,@End_Date DATETIME
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
    
    IF @strRegion = '' OR @strRegion IS NULL 
    BEGIN
    
    SELECT 
       license.region as [Region]
      ,license.sub_region AS [Sub Region]
      ,license.license_name AS [License Name]
      ,RRR.[agent] AS [Agent]
      ,RRR.[deal_number] AS [Deal Number]
      ,REPLACE(CONVERT(VARCHAR(10), RRR.[created_date], 111), '/', '-') AS [Created Date]
      ,REPLACE(CONVERT(VARCHAR(7), RRR.contract_start_date, 111),'/','-')     AS [Contract Year Month Start] 
      ,REPLACE(CONVERT(VARCHAR(10), RRR.[contract_start_date], 111), '/', '-') AS [Contract Start]
      ,REPLACE(CONVERT(VARCHAR(10), RRR.[contract_end_date], 111), '/', '-') AS [Contract End]
      ,RRR.[comm] AS [Total @ 7%]
      ,RRR.[entity_type] AS [Entity Type]
      ,user_registration.user_preferred_name + ' ' + user_registration.user_surname AS [Created By]
      ,RRR.units AS Units
	 	
  FROM 
		[boss].[dbo].[reports_rental_recognition] AS RRR
  INNER JOIN 
		license on RRR.license_id = license.license_id
  INNER JOIN
		user_registration ON RRR.created_by = user_registration.registration_id
  WHERE
		(RRR.contract_start_date >= @Begin_Date)
  AND
		(RRR.contract_start_date <= @End_Date)
  
  ORDER BY license.region ASC	
    
    
    END
    
    ELSE
    
    BEGIN
    
	SELECT 
       license.region as [Region]
      ,license.sub_region AS [Sub Region]
      ,license.license_name AS [License Name]
      ,RRR.[agent] AS [Agent]
      ,RRR.[deal_number] AS [Deal Number]
      ,REPLACE(CONVERT(VARCHAR(10), RRR.[created_date], 111), '/', '-') AS [Created Date]
      ,REPLACE(CONVERT(VARCHAR(7), RRR.contract_start_date, 111),'/','-')     AS [Contract Year Month Start] 
      ,REPLACE(CONVERT(VARCHAR(10), RRR.[contract_start_date], 111), '/', '-') AS [Contract Start]
      ,REPLACE(CONVERT(VARCHAR(10), RRR.[contract_end_date], 111), '/', '-') AS [Contract End]
      ,RRR.[comm] AS [Total @ 7%]
      ,RRR.[entity_type] AS [Entity Type]
      ,user_registration.user_preferred_name + ' ' + user_registration.user_surname AS [Created By]
      ,RRR.units AS Units
	 	
  FROM 
		[boss].[dbo].[reports_rental_recognition] AS RRR
  INNER JOIN 
		license on RRR.license_id = license.license_id
  INNER JOIN
		user_registration ON RRR.created_by = user_registration.registration_id
  WHERE
		(license.region = @strRegion)
   AND
		(RRR.contract_start_date >= @Begin_Date)
  AND
		(RRR.contract_start_date <= @End_Date)
  
  ORDER BY license.license_name ASC	
  
  END
  
END




