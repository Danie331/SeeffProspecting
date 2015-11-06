create database spatial_web_app
go

use spatial_web_app
go

create table dbo.exception_log
(
	exception_log_id int identity(1,1) primary key not null,
	context varchar(255) not null,
	[user] uniqueidentifier not null,
	created datetime not null,
	raw_exception nvarchar(max) not null,
	state_object_json nvarchar(max) null	
);