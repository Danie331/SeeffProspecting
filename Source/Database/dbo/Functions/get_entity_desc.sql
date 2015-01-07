

-- =============================================
-- Author:		Adam Roberts
-- Create date: 2011-11-29
-- Description:	Return the display text for an Entity
-- =============================================
CREATE FUNCTION [dbo].[get_entity_desc] 
(
	-- Add the parameters for the function here
	@entity_type char(1),
	@entity_id INT
)
RETURNS VARCHAR(250)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @Desc VARCHAR(250)

    IF @entity_type = 'I'
      BEGIN
	    SELECT @Desc = (SELECT ([user_preferred_name] + ' ' + [user_surname])
                          FROM [boss].[dbo].[user_registration]
                         WHERE [registration_id] = @entity_id);
      END
    ELSE
      BEGIN  
        SELECT @Desc = (SELECT [partnership_name]
                          FROM [boss].[dbo].[partnership]
                         WHERE [partnership_id] = @entity_id);
      END;
	RETURN @Desc 
END


