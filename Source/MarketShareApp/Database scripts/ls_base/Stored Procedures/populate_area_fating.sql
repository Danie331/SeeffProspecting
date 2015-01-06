use ls_base
go

create procedure dbo.populate_area_fating
as
begin
	truncate table area_fating;
	insert area_fating (area_id, fated, unfated)
	select distinct b.seeff_area_id,
	(select count(*) from dbo.base_data where seeff_area_id = b.seeff_area_id
								and fated is not null and market_share_type is not null) fated,
	(select count(*) from dbo.base_data where seeff_area_id = b.seeff_area_id
								 and (fated is null OR market_share_type is null)) unfated
	from base_data b
end;