





-- ===================================================
-- Author:		GW Swanepoel
-- Create date: 2012-06-18
-- Description:	Returns totals for full period (YTD)
--				for Rental Recognition Report
--				per month
--				from table: reports_rental_recognition
-- ===================================================
CREATE PROCEDURE [dbo].[spMIS_RentRecog_Lic_YTDSummary]
	-- Add the parameters for the stored procedure here
	@Lic_ID INT
	,@Begin_Date NVARCHAR(20)
	,@End_Date NVARCHAR(20)
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
		(CONTRACT_START_DATE >= @Begin_Date)
AND
		(CONTRACT_START_DATE <= @End_Date)
AND
		(entity_type = @EntityType)				
GROUP BY 
		agent
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
			(CONTRACT_START_DATE >= @Begin_Date)
	AND
			(CONTRACT_START_DATE <= @End_Date)
	AND
			(entity_type = @EntityType)				
	GROUP BY 
			agent
	ORDER BY [Recognition Amount] DESC
	END
	
END





