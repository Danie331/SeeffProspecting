
CREATE function dbo.get_province_id(@province_name varchar(max))
returns int
as
begin
	DECLARE @prov_id int;
	set @prov_id = (select seeff_area_id from dbo.province_id_lookup
					where lightstone_prov_name = @province_name);
	return @prov_id;
end;