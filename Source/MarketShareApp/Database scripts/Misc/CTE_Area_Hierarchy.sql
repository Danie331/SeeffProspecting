
-- Creates a flattened view of the area hierarchy
SELECT a1.areaId ProvinceAreaId,a1.areaName ProvinceName,
		a2.areaId RegionAreaId,a2.areaName RegionName, 
		a3.areaId CityTownAreaId,a3.areaName CityTownName, 
		a4.areaId AreaAreaId,a4.areaName AreaName,
		a5.areaId SuburbAreaId,a5.areaName SuburbName
		 FROM seeff.[dbo].[area] a1  
left join seeff.[dbo].[area] a2 on a2.areaParentId = a1.areaId
left join seeff.[dbo].[area] a3 on a3.areaParentId = a2.areaId
left join seeff.[dbo].[area] a4 on a4.areaParentId = a3.areaId
left join seeff.[dbo].[area] a5 on a5.areaParentId = a4.areaId
where a1.areaParentId = 1
order by a1.areaId
