  alter table boss.dbo.user_registration
  add prospecting_areas varchar(max) null,
	  prospecting_credits int default 0 not null;
