
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[achiever_type]
(
	-- Add the parameters for the function here
	@life_time bit, 
	@platinum bit,
	@ordinary bit
)
RETURNS int
AS
BEGIN
	-- Declare the return variable here
	-- DECLARE <@ResultVar, sysname, @Result> <Function_Data_Type, ,int>

	IF @life_time = 1 
	BEGIN
	  RETURN 1
	END
	IF @platinum = 1 
	BEGIN
	  RETURN 2
	END
	IF @ordinary = 1 
	BEGIN
	  RETURN 3
	END
	RETURN 0
END

