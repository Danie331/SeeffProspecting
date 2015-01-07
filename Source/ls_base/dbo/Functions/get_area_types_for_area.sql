
CREATE function dbo.get_area_types_for_area (@area_id int)
returns @results table
(
	area_type varchar(1) not null
)
as
begin
	insert @results
	select area_type from seeff.dbo.kml_area
	where area_id = @area_id
	group by area_type
	return;
end;
