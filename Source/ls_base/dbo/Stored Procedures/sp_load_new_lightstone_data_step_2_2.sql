create procedure sp_load_new_lightstone_data_step_2_2
as
begin
	-- Step 7:Populate property addresses.
	print 'Populating property addresses';
	while (exists(select property_address from new_data_temp where property_address is null))
	begin
		begin try
			 select dbo.sp_update_property_addresses ('ls_base.dbo.new_data_temp', 'localhost', 'test');
		end try
		begin catch		
			WAITFOR DELAY '00:00:05';
		end catch
	end;
end;