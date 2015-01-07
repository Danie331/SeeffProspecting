
CREATE function dbo.reverse_comma_delimited_string (@input varchar(max))
returns varchar(max)
as
begin
	DECLARE @source VARCHAR(MAX)
	DECLARE @dest VARCHAR(MAX)
	DECLARE @lenght INT 

	SET @source = @input;
	SET @dest = ''

	WHILE LEN(@source) > 0
	BEGIN
		IF CHARINDEX(',', @source) > 0
		BEGIN
			SET @dest = SUBSTRING(@source,0,CHARINDEX(',', @source)) + ',' + @dest
			SET @source = LTRIM(RTRIM(SUBSTRING(@source,CHARINDEX(',', @source)+1,LEN(@source))))
		END
		ELSE
		BEGIN
			SET @dest = @source + ',' + @dest
			SET @source = ''
		END
	END
	SET @dest = LEFT(@dest, LEN(@dest) - 1);
	
	return @dest;
end;