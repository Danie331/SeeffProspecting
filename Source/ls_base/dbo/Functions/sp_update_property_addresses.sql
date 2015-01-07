CREATE FUNCTION [dbo].[sp_update_property_addresses]
(@table_name NVARCHAR (255), @curr_server NVARCHAR (255), @curr_env NVARCHAR (255))
RETURNS INT
AS
 EXTERNAL NAME [PropertyAddressUpdater].[GetPropertyAddress.TestGetAddress].[PopulatePropertyAddresses]

