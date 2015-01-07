-- =============================================
-- Author:		<Author,,Peter>
-- Create date: <Create Date, 2013/06/07 ,>
-- Description:	<Description, Calculates price adjusted ,>
-- =============================================
CREATE FUNCTION [dbo].[CalculateSearchPermLand]
(
	-- Add the parameters for the function here
	 @searchPriceAdjusted numeric,
	 @searchERFSize varchar(50),
	 @searchERFUnit numeric
)
RETURNS money
AS
BEGIN

	-- Declare the return variable here
	DECLARE @searchPerm2Land money
	DECLARE @searchERSizeMoney money
	DECLARE @CONS_valueOne as decimal

	Set @CONS_valueOne = 1000000

	IF @searchERFSize = ' '
	BEGIN
		RETURN 0
	END
	
	IF @searchERFSize = ''
	BEGIN
		RETURN 0
	END
	
		IF cast(@searchERFSize as decimal) = 0
	BEGIN
		RETURN 0
	END

	IF @searchERFSize = 'NULL'
	BEGIN
		RETURN 0
	END


	Set @searchERSizeMoney = cast(@searchERFSize AS decimal)
	
	-- Add the T-SQL statements to compute the return value here

	IF @searchERFUnit = 1 --if it is m2 then do standard division
	BEGIN
		SET @searchPerm2Land = @searchPriceAdjusted / @searchERSizeMoney
		return @searchPerm2Land
		return isNull(@searchPerm2Land,0)
	END


	IF @searchERFUnit = 2 --If in hectares hectares 10000 m2
	BEGIN
		SET @searchPerm2Land = @searchPriceAdjusted / (@searchERSizeMoney * 10000)
		return isNull(@searchPerm2Land,0)
	END

	IF @searchERFUnit = 3 --If in Km convert to 1000000 m2
	BEGIN
		SET @searchPerm2Land = @searchPriceAdjusted / (@searchERSizeMoney)
		return @searchPerm2Land
		--return isNull(@searchPerm2Land,0)
	END

	-- Return the result of the function

	RETURN isNull(@searchPerm2Land,0)

END