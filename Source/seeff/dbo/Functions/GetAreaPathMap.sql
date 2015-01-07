-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[GetAreaPathMap] 
(
	-- Add the parameters for the function here
	@propertyAreaId INT,
	@areaTypeId INT
)
RETURNS INT 
AS
BEGIN
	-- Declare the return variable here
	DECLARE @SPath VARCHAR(2000)
	DECLARE @areaAreaId nvarchar(50)
	
	-- Add the T-SQL statements to compute the return value here
	SELECT @SPath = sPath FROM AreaMap WHERE fkAreaId = @propertyAreaId 
	SELECT @areaAreaId = areaId FROM dbo.area WHERE patindex('%|'+CAST(areaid AS varchar(10))+'|%',  @SPath) > 0 AND fkAreaTypeId = @areaTypeId
	
	
	RETURN isNull(@areaAreaId,'-1')

END

GO
GRANT EXECUTE
    ON OBJECT::[dbo].[GetAreaPathMap] TO [IIS APPPOOL\smartadmin.seeff.com]
    AS [dbo];

