
-- =============================================
-- Author:		Adam Roberts
-- Create date: 2014-03-27
-- Description:	Execute SQL passed in
-- =============================================
Create PROCEDURE [dbo].[exec_sql] 
   @sql VARCHAR(MAX)	
AS
BEGIN
	SET NOCOUNT ON;

	EXEC (@sql)
END

