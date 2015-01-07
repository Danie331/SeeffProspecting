
 

 

-- =============================================

-- Author:        Adam Roberts

-- Create date: 2011/09/29

-- Description:   This function returns the next webref no on the

--              seeff db, it is used but the PropCtrlSync Service 

-- =============================================

CREATE FUNCTION [dbo].[GET_NEXT_WEB_REF]()

RETURNS INT

AS

BEGIN

      -- Declare the return variable here

      DECLARE @web_ref INT;

 

      -- Add the T-SQL statements to compute the return value here

      SELECT @web_ref = (SELECT MAX(propertyReference) + 1

                       FROM seeff.dbo.property)

 

      -- Return the result of the function

      RETURN @web_ref

END

 

 

