use ls_base
go

create procedure sp_load_new_lightstone_data
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

	 -- Step 5: re-populate the area_layer table: ensure the latest seeff database exists
	 -- Moved to pproduction server to be run before backup.
	 -- execute dbo.sp_populate_area_layer_tbl;

	-- Step 6: Run code to populate the remaining records with area id's
	declare @row_count INT;
	set @row_count = (select count(*) from new_data_temp);
	print 'Number of new records for insert: ' + cast(@row_count as varchar);
	execute dbo.update_seeff_area_ids;

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

	-- 1266268820131001
	-- Step 8: insert into base_data the remaining records
	insert into base_data ([ls_area_id]
      ,[seeff_lic_area]
      ,[property_id]
      ,[iregdate]
      ,[ipurchdate]
      ,[title_deed_no]
      ,[non_gar_props_on_title]
      ,[purch_price]
      ,[erf_key]
      ,[ea_code]
      ,[suburb]
      ,[munic_name]
      ,[province]
      ,[property_type]
      ,[ss_fh]
      ,[property_currently_in_pvt_hands]
      ,[private_registration]
      ,[bonded_transfer]
      ,[suburb_id]
      ,[erf_size]
      ,[buyer_name]
      ,[seller_name]
      ,[property_address]
      ,[street_or_unit_no]
      ,[ls_resflg]
      ,[estate_name]
      ,[buyer_name_2]
      ,[multibuyer]
      ,[seeff_suburb]
      ,[seeff_suburb_id]
      ,[seeff_suburb_type]
      ,[seeff_lic_id]
	  ,[unique_id]
	  ,[seeff_deal]
	  ,[seeff_area_id]
      ,[x]
      ,[y]
	  ,[erf_no]
	  ,[portion_no]) 
	select [ls_area_id]
      ,[seeff_lic_area]
      ,[property_id]
      ,[iregdate]
      ,[ipurchdate]
      ,[title_deed_no]
      ,[non_gar_props_on_title]
      ,[purch_price]
      ,[erf_key]
      ,[ea_code]
      ,[suburb]
      ,[munic_name]
      ,[province]
      ,[property_type]
      ,[ss_fh]
      ,[property_currently_in_pvt_hands]
      ,[private_registration]
      ,[bonded_transfer]
      ,[suburb_id]
      ,[erf_size]
      ,[buyer_name]
      ,[seller_name]
      ,[property_address]
	  ,[street_or_unit_no]
      ,[ls_resflg]
      ,[estate_name]
      ,[buyer_name_2]
      ,[multibuyer]
      ,[seeff_suburb]
      ,[seeff_suburb_id]
      ,[seeff_suburb_type]
      ,[seeff_lic_id]
	  ,[unique_id]
	  	  ,[seeff_deal]
	  ,[seeff_area_Id]
      ,[x]
      ,[y]
	  ,[erf_no]
	  ,[portion_no] 
	 from new_data_temp;

	 -- This step is very NB!
	 -- Step 9: Ensure that every seeff transaction is updated with agency if for seeff
	 update base_data
	 set agency_id = (select agency_id from agency where agency_name = 'Seeff')
	 where seeff_deal = 1;

	 -- Step 10: Recalculate the area fating table
	 exec dbo.populate_area_fating;

	 -- Step 11: Run the SP to populate the area_parent_lookup table
	 -- Moved to pproduction server to be run before backup.
	 -- exec [dbo].[sp_generate_area_parent_lookup_table];

	 -- Step 12.1: fate all transactions < 250000 as 'Other'
	 update base_data
	 set fated = 1, market_share_type = 'O'
	 where purch_price < 250000 and fated is null;

	 -- Step 12.2: fate all transactions with no sale price as 'Other'
	 update base_data
	 set fated = 1, market_share_type = 'O'
	 where purch_price is null and fated is null;

	-- Step 13.1: Set the flag on records that are part of transactions that include multiple items (same title_deed_no and purch_price)
	;WITH multiple_props (title_deed, price) as
	(
	select title_deed_no, purch_price from base_data
	where purch_price is not null and seeff_area_id > -1
	group by title_deed_no, purch_price
	having count(*) > 1
	)
	update base_data 
	set sale_includes_others_flag = 1
	from multiple_props where title_deed_no = title_deed and purch_price = price;

	-- Step 13.2: Ensure that all records that have an existing parent-child relation are mapped to new title_deeds with the same prop Id's
	
	-- Step 14: We can auto fate seeff listings here
	 update bd
	 set fated = 1, market_share_type = (case tr.sps_transaction_division
										when 'Residential' then 'R'
										when 'Agriculture' then 'A'
										when 'Commercial' then 'C'
										when 'Development' then 'D'
										end)
	 from base_data bd
	 join boss.dbo.sps_transaction tr on property_id = tr.sps_property_id AND iregdate = tr.lightstone_reg_date
	 where fated IS NULL and seeff_deal = 1;

	-- Step 15: delete the temporary table
	drop table new_data_temp;  
end
GO