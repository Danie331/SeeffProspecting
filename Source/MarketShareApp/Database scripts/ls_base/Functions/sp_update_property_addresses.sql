use ls_base
go

create function dbo.sp_update_property_addresses(@table_name nvarchar(255),@curr_server nvarchar(255), @curr_env nvarchar(255))
returns int
as external name PropertyAddressUpdater.[GetPropertyAddress.TestGetAddress].[PopulatePropertyAddresses]
GO
