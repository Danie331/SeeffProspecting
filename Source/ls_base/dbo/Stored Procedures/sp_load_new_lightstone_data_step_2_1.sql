create procedure sp_load_new_lightstone_data_step_2_1
as
begin
	-- Step 6: Run code to populate the remaining records with area id's
	declare @row_count INT;
	set @row_count = (select count(*) from new_data_temp);
	print 'Number of new records for insert: ' + cast(@row_count as varchar);
	execute dbo.update_seeff_area_ids;
end;