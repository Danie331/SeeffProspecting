
-- =============================================
-- Author:		GW Swanepoel
-- Create date: 2012-11-11 15:00
-- Description:	Stored procedure that
--				backups BOSS database
--				to hardcoded directory
--				(in this examples: 
--				C:\BOSS_Backups\BOSS_
--				)
-- =============================================
CREATE PROCEDURE [dbo].[spMIS_Backup_BOSS] 
	-- Add the parameters for the stored procedure here
	
	@FileName NVARCHAR(100) OUTPUT
	,@IsMonthEnd NVARCHAR(100) OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	
	SET NOCOUNT ON;

	 -- Insert statements for procedure here
		DECLARE @MyPath NVARCHAR(MAX)
			DECLARE @DBSaveName NVARCHAR(MAX)
				DECLARE @strSQL NVARCHAR(MAX)
				
		SET @DBSaveName = (
					SELECT REPLACE(('BOSS_' + REPLACE(CONVERT(NVARCHAR(20),GETDATE(),120),':','') +																'.bak'),' ','_')
						  )			
	
				SELECT @MyPath = (SELECT 'C:\SQL Backup Folder\' + @DBSaveName +'')					
		BEGIN
		SET @strSQL = 'BACKUP DATABASE boss TO DISK='''+ @MyPath + ''''		
			PRINT @strSQL
				EXEC (@strSQL);
	END

		SET @FileName = (SELECT @MyPath)

		SET @IsMonthEnd = ( SELECT 
											COUNT(*) 
									FROM 
											reports_month_end_cutoff_dates 
									WHERE 
											Cut_off_Date = CONVERT(VARCHAR(10),GETDATE(),121)
								   )	
		RETURN;

END

