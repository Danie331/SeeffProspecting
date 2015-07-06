
insert into [activity_type]
values ('Change Of Ownership', 1, 1);

alter table prospecting_property
add latest_reg_date varchar(8) null;

-- when user takes the necessary action, update lightstone_reg_date to the new date, update updated_date and set new date to null.
-- Then sever ties between person_property and compnay_property relationships, and prospect.

with cte (property_id, new_reg_date, rn) as (
select pp.lightstone_property_id, bd.iregdate, ROW_NUMBER() over (partition by bd.property_id order by cast(bd.iregdate as date) desc) as rn
from ls_base.dbo.base_data bd
join prospecting_property pp
on pp.lightstone_property_id = bd.property_id
where (isdate(bd.iregdate) = 1 and isdate(pp.lightstone_reg_date) = 1) and
cast(bd.iregdate as DATE) > cast(pp.lightstone_reg_date as date) )

select * into #temp from cte where rn = 1;
update pp
set pp.latest_reg_date = t.new_reg_date
from prospecting_property pp
join #temp t on t.property_id = pp.lightstone_property_id;




