USE [ls_base]
GO

/****** Object:  StoredProcedure [dbo].[sp_load_new_lightstone_data_step_2_1]    Script Date: 2014-08-20 06:34:03 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

create procedure [dbo].[sp_load_new_lightstone_data_step_2_1]
as
begin
	-- Step 6: Run code to populate the remaining records with area id's
	declare @row_count INT;
	set @row_count = (select count(*) from new_data_temp);
	print 'Number of new records for insert: ' + cast(@row_count as varchar);
	execute dbo.update_seeff_area_ids;
end;
GO


