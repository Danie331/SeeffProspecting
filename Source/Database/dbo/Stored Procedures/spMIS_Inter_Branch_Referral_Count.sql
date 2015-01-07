
-- =============================================
-- Author:		Michael Scott
-- Create date: 28/02/2013
-- Description:	Inter Branch Referral Count Sheet for the Smart Pass Overview Report.
-- =============================================
CREATE PROCEDURE [dbo].[spMIS_Inter_Branch_Referral_Count]

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT YEAR([created_date]) AS [Year], MONTH([created_date]) AS [Month]
      ,COUNT(*) AS [No.]
  FROM [boss].[dbo].[smart_pass]
WHERE [license_id_to] <> [license_id_from] 
GROUP BY YEAR([created_date]), MONTH([created_date])
ORDER BY YEAR ([created_date]), MONTH([created_date])

END


