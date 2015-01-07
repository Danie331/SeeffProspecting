CREATE FUNCTION [dbo].[sp_update_seeff_area_ids]
(@table_name NVARCHAR (255), @curr_server NVARCHAR (255), @curr_env NVARCHAR (255))
RETURNS INT
AS
 EXTERNAL NAME [SeeffAreaIdUpdater].[kml_area_lookup_sql_lib.Core].[UpdateSeeffAreaIds]

