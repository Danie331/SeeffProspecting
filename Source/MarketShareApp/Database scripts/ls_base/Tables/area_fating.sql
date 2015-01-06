use ls_base 
go

create table dbo.area_fating
(
	area_fating_id int primary key identity(1,1) not null,
	area_id int not null,
	fated int not null,
	unfated int not null
)
go