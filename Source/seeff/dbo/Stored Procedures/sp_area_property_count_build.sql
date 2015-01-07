-- =============================================
-- Author:		scott murray
-- Create date: 29/01/2014
-- Description:	counts how many properties there 
--				are under a specific area
-- =============================================
CREATE PROCEDURE [dbo].[sp_area_property_count_build] 
@startTime DATETIME = NULL,
	 @endTime DATETIME = NULL
AS
SET @startTime = GETDATE()
--if the table exists then drop it
if exists (select * from dbo.sysobjects where id = object_id(N'[area_property_count]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table area_property_count

CREATE TABLE area_property_count
(
area_property_count_id int IDENTITY(1,1) PRIMARY KEY ,
fkAreaId int not null,
areaName varchar(255) not null,
fkAreaTypeId int not null,
fkParentAreaId int,
buy_count bigint,
rent_count bigint,
updated_date DATETIME NOT NULL DEFAULT getdate(),
);

BEGIN

INSERT INTO [seeff].[dbo].[area_property_count]
(
fkAreaId,
areaName,
fkAreaTypeId,
fkParentAreaId,
buy_count,
rent_count
)

SELECT
seeff.dbo.area.areaId,
seeff.dbo.area.areaName,
seeff.dbo.area.fkAreaTypeId,
seeff.dbo.area.areaParentId,
(SELECT COUNT(propertyId)
FROM property
JOIN AreaMap
ON property.fkAreaId = AreaMap.fkAreaId
WHERE property.propertyActive = 1
AND property.fkActionId = 2
AND PATINDEX('%|' + CAST(seeff.dbo.area.areaId AS varchar) + '|%', [AreaMap].[sPath]) > 0),
(SELECT COUNT(propertyId)
FROM property
JOIN AreaMap
ON property.fkAreaId = AreaMap.fkAreaId
WHERE property.propertyActive = 1
AND property.fkActionId = 3
AND PATINDEX('%|' + CAST(seeff.dbo.area.areaId AS varchar) + '|%', [AreaMap].[sPath]) > 0)
from seeff.dbo.area

/*SET THE END TIM WHEN QUERY FINISHES EXECUTING*/
	SET @endTime = GETDATE()

	/*Print start and end time for logging*/
	print 'START TIME: ' + CAST(@startTime AS VARCHAR)
	print 'END TIME: ' + CAST(@endTime AS VARCHAR)
	print 'TOTAL EXECUTION TIME IN SECONDS: ' +  CAST(DATEDIFF(ss,@startTime,@endTime) AS VARCHAR)


END
