


CREATE procedure [dbo].[sp_load_new_lightstone_data_step_4]
as
begin

	 -- This step is very NB!
	 -- Step 9: Ensure that every seeff transaction is updated with agency if for seeff
	 update base_data
	 set agency_id = (select agency_id from agency where agency_name = 'Seeff')
	 where seeff_deal = 1;

	 -- Step 10: Recalculate the area fating table
	 exec dbo.populate_area_fating;

	 -- Step 11: Run the SP to populate the area_parent_lookup table
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
end;


