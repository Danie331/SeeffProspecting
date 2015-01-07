-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[GetAreaParentName] 
(
	-- Add the parameters for the function here
	@areaIdVar INT
)
RETURNS varchar(100)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @areaParentName nvarchar(100)
	DECLARE @areaParentId nvarchar(100)

	-- Add the T-SQL statements to compute the return value here
	SELECT @areaParentId = areaParentId FROM dbo.area WHERE areaId = @areaIdVar
	SELECT @areaParentName = areaName FROM dbo.area WHERE areaId = @areaParentId
		
	RETURN isNull(@areaParentName,'')

END
