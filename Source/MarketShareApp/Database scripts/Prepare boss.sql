use boss
go

-- create the permissions for user
alter table dbo.user_registration
add ms_area_permissions nvarchar(MAX) null
go

-- now populate permissions for user here