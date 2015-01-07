
-- =============================================
-- Author:		GW Swanepoel
-- Create date: 20120709
-- Description:	See the google sheet titled
--				ICAS Recon 20120707
--				Query written by Adam Roberts
-- =============================================
CREATE PROCEDURE [dbo].[spMIS_ICAS_Subscribers]
	-- Add the parameters for the stored procedure here
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
  SELECT	
			[license_id] AS [License ID]
			,[license_name] AS [License Name]
			,[region] AS [Region]
			,[sub_region] AS [Sub Region]
			,CASE 
				WHEN [icas] = 1 THEN 'Yes'
				WHEN [icas] = 0 THEN 'No'
				ELSE ''
				END AS [ICAS Subscriber?]
  FROM 
			[boss].[dbo].[license]
  WHERE 
			[status] LIKE 'A'
  ORDER BY 
			[icas]
			, [license_name]
END

