
CREATE function [dbo].[ufn_get_area_parent_lookup_json]()
returns varchar(max)
as
begin
	declare @temp varchar(max) = '[';

	select @temp = @temp + '{Name: "' + [name] + '", id: "' + cast(area_id as varchar) + '", type: "' + area_type + '"},' 
					from  dbo.area_parent_lookup;

	set @temp = substring(@temp, 1, len(@temp) - 1);
	set @temp = @temp + ']';

	return @temp;
end;