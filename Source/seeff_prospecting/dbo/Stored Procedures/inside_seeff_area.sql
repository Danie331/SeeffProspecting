

CREATE procedure [dbo].[inside_seeff_area](@lat decimal(13, 8), @lng decimal(13, 8), @seeff_area_id int)
as
begin
	-- Replace the function with [seeff_spatial].[dbo].[point_in_area] when dev is moved to 2014
	DECLARE @result int = 0;
    SELECT @result = [seeff_spatial].[dbo].[point_in_area] (
                     @lat, @lng, @seeff_area_id)
	return @result;
end;



