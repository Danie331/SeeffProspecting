CREATE procedure [dbo].[find_area_id](@lat decimal(13, 8), @lng decimal(13, 8) , @area_type varchar(1), @province_id int)
as
begin
 declare @result int = 0;

  create table #tmp 
  (
   area_id int,
   poly_coords varchar(max)
  );

  if @province_id <> null 
  begin
   insert into #tmp (area_id, poly_coords)
   select al.prospecting_area_id, al.[formatted_poly_coords] from [dbo].[prospecting_area_layer] al
   where al.area_type =  @area_type and al.province_id = @province_id;
  end 
  else
  begin
   insert into #tmp (area_id, poly_coords)
   select al.prospecting_area_id, al.[formatted_poly_coords] from [dbo].[prospecting_area_layer] al
   where al.area_type =  @area_type;
  end

 DECLARE @area_id int, @poly_coords varchar(max);
 DECLARE poly_cursor CURSOR FOR 
 SELECT area_id, poly_coords FROM #tmp;

 OPEN poly_cursor
 FETCH NEXT  FROM poly_cursor INTO @area_id, @poly_coords
 WHILE @@FETCH_STATUS = 0
 BEGIN
  SET @result = 0;
  BEGIN TRY
   SET @result = [dbo].[point_inside_poly](@lat, @lng, @poly_coords);
  END TRY
  BEGIN CATCH
  END CATCH

  IF @result = 1
  BEGIN
   drop table #tmp;
   CLOSE poly_cursor;
   DEALLOCATE poly_cursor;

   RETURN @area_id;
  END

   FETCH NEXT FROM poly_cursor 
  INTO @area_id, @poly_coords
 END 
 drop table #tmp;
 CLOSE poly_cursor;
 DEALLOCATE poly_cursor;

 return @result;
end