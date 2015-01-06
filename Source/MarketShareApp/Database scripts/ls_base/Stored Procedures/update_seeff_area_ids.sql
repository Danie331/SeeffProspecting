use ls_base
go

create procedure dbo.update_seeff_area_ids
as
begin
	DECLARE @result int;
	DECLARE @unique_id nvarchar(50), @latitude decimal(13, 8), @longitude decimal(13, 8), @prov_id int;
	DECLARE temp_cursor CURSOR FOR 
	SELECT unique_id, Y, X, dbo.get_province_id(province) FROM new_data_temp;

	OPEN temp_cursor
	FETCH NEXT  FROM temp_cursor INTO @unique_id, @latitude, @longitude, @prov_id
	WHILE @@FETCH_STATUS = 0
	BEGIN
		exec @result = dbo.[find_area_id] @lat = @latitude, @lng = @longitude, @area_type = 'R', @province_id = @prov_id;
		update new_data_temp
		set seeff_area_id = CASE  
							WHEN @result > 0 THEN @result
							WHEN @result = 0 THEN -1
							END
		WHERE unique_id = @unique_id;

		FETCH NEXT FROM temp_cursor 
		INTO @unique_id, @latitude, @longitude, @prov_id
	END;

	CLOSE temp_cursor;
	DEALLOCATE temp_cursor;
end;