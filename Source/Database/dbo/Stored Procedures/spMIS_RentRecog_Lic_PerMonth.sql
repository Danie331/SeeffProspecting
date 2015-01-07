



-- ===================================================
-- Author:		GW Swanepoel
-- Create date: 2012-06-18
-- Description:	Returns totals per month
--				for Rental Recognition Report
--				per month
--				from table: reports_rental_recognition
-- ===================================================
CREATE PROCEDURE [dbo].[spMIS_RentRecog_Lic_PerMonth]
	-- Add the parameters for the stored procedure here
	@Lic_ID INT
	,@Year_Start INT
	,@Month_Start INT
	,@EntityType NVARCHAR(20)
	,@SortType NVARCHAR(20)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
    
   IF @SortType LIKE 'Units'
    BEGIN
    
    
	SELECT
		  DENSE_RANK() OVER (Order by SUM([units]) desc) AS [#]
		, [agent] AS [Agent] 
        , SUM([units]) AS [Total Units]
  FROM 
		[boss].[dbo].[reports_rental_recognition] AS RRR
WHERE 
		(license_id = @Lic_ID)
AND
		(YEAR(CONTRACT_START_DATE) = @Year_Start)
AND
		(MONTH(CONTRACT_START_DATE) = @Month_Start)
AND
		(entity_type = @EntityType)				
GROUP BY 
		REPLACE(CONVERT(VARCHAR(7), RRR.contract_start_date, 111),'/','-')
		,agent
ORDER BY [Total Units] DESC
END

IF @SortType LIKE 'Comm'
	BEGIN
	
	SELECT
			  DENSE_RANK() OVER (Order by SUM([comm]) desc) AS [#]
			, [agent] AS [Agent] 
			, SUM([comm]) AS [Recognition Amount]
	
	  FROM 
			[boss].[dbo].[reports_rental_recognition] AS RRR
	WHERE 
			(license_id = @Lic_ID)
	AND
			(YEAR(CONTRACT_START_DATE) = @Year_Start)
	AND
			(MONTH(CONTRACT_START_DATE) = @Month_Start)
	AND
			(entity_type = @EntityType)				
	GROUP BY 
			REPLACE(CONVERT(VARCHAR(7), RRR.contract_start_date, 111),'/','-')
			,agent
	ORDER BY [Recognition Amount] DESC
	END
	
END



