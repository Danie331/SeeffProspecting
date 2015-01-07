create procedure sp_load_new_lightstone_data_step_1
as
begin
	-- Step 1: create a new temporary table to store the new records
    SELECT * INTO new_data_temp FROM seeff_deeds.dbo.SEEFF_Deeds_Monthly;
	-- Step 1.1: add the area id column that will be populated with the a Seeff area id
	ALTER TABLE new_data_temp
	add [seeff_area_id] int null,
	 [unique_id] nvarchar(50) null,
	 [seeff_deal] bit null,
	 [property_address] varchar(255) null,
	 [street_or_unit_no] varchar(255) null,
	 [erf_no] int null,
	 [portion_no] int null;

	 alter table new_data_temp
	 add new_data_temp_id int identity(1,1) not null;
	 alter table new_data_temp
	 ADD CONSTRAINT pk_new_data_temp_id PRIMARY KEY (new_data_temp_id);

	-- Step 2: delete rows from this temp table that have a matching unique_id from ls_base, as these records exist in ls_base
	delete temp 
	from new_data_temp temp
	join
	base_data b on temp.property_id = b.property_id and temp.iregdate = b.iregdate;

	 -- Step 3: update the unique_id column in base_data for the new records
	 update new_data_temp
	 set unique_id = concat(cast(property_id as nvarchar), cast(iregdate as nvarchar))

	 -- Step 4: if the unique_id of any new records matches the unique id obtained in the boss.sps_transaction tbl
	 --			then we must flag the record as a seeff deal
	 update tmp
	 set tmp.seeff_deal = 1
	 from new_data_temp tmp
	 join boss.dbo.sps_transaction tr on tmp.property_id = tr.sps_property_id AND tmp.iregdate = tr.lightstone_reg_date; 
end;