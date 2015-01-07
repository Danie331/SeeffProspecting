
-- =============================================
-- Author:		Michael Scott
-- Create date: 28/02/2013
-- Description:	Returns the All Referrals Count Sheet for the Smart Pass Overview Report.
-- =============================================
CREATE PROCEDURE [dbo].[spMIS_All_Referrals_Count]
	-- Add the parameters for the stored procedure here

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT YEAR([created_date]) AS [Year], MONTH([created_date]) AS [Month]
      ,COUNT(*) AS [No.]
  FROM [boss].[dbo].[smart_pass]
  GROUP BY YEAR([created_date]), MONTH([created_date])
  ORDER BY YEAR([created_date]), MONTH([created_date])

END

