
use ls_base
go

create table dbo.province_id_lookup
(
	province_id_lookup int identity(1,1) primary key not null,
	lightstone_prov_name varchar(max) not null,
	seeff_prov_name varchar(max) not null,
	seeff_area_id int not null
)
go

insert dbo.province_id_lookup (lightstone_prov_name, seeff_prov_name, seeff_area_id)
values ('WESTERN CAPE', 'Western Cape', 2),
('GAUTENG','Gauteng',11),
('FREE STATE','Free State',12),
('KWAZULU NATAL','Kwazulu Natal',13),
('LIMPOPO','Limpopo',14),
('MPUMALANGA','Mpumalanga',15),
('NORTH WEST','North West Province',16),
('EASTERN CAPE','Eastern Cape',17),
('NORTHERN CAPE','Northern Cape',18);