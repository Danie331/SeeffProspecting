USE [ls_base]
GO

/****** Object:  StoredProcedure [dbo].[sp_load_new_lightstone_data_step_2_2]    Script Date: 2014-08-20 06:34:09 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

create procedure [dbo].[sp_load_new_lightstone_data_step_2_2]
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
GO


