
-- Ensure that the database has the schema shown below, when generating the new records for INSERT ensure that they too contain this schema.
-- See the base_data_schema.sql file for full schema.
use ls_base
go

alter table dbo.base_data
add agency_id int null
go

alter table dbo.base_data
add fated bit null
go

alter table dbo.base_data
add market_share_type varchar(1) null
go

ALTER TABLE base_data ADD base_data_id int identity(1,1) not null
GO
ALTER TABLE base_data
add CONSTRAINT pk_base_data_id primary key(base_data_id)
GO

alter table base_data
add erf_no int null,
	portion_no int null;


alter table base_data
add parent_property_id int null;

alter table base_data
add sale_includes_others_flag bit null;
