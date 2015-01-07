

-- =============================================
-- Author:		Scott Murray
-- ALTER  date: 09.12.2013
-- Description:	This proc inserts a listing into the search table
-- =============================================
CREATE PROCEDURE [dbo].[sp_search_table_property_insert]
	-- Add the parameters for the stored procedure here
		@webRef int = NULL
AS


DELETE FROM search WHERE [seeff].[dbo].[search].[searchReference] = @webRef

BEGIN
--Select columns for primary insert
	INSERT INTO [seeff].[dbo].[search]
	(
	[searchCountry],
	[searchProvince] ,
	[searchRegion],
	[searchCity],
	[searchArea] ,
	[searchSuburb],
	[searchAreaName], 
	[fkPropertyId],  
	[fkAgentId],
	[fkBranchId], 
	[fkAreaParentId], 
	[fkAreaId], 
	[fkCategoryId], 
	[fkPropertyTypeId], 
	[searchPropertyTypeName], 
	[fkActionId], 
	[fkGolfEstateId], 
	[searchReference],
	[searchPrice], 
	[fkCurrencyId], 
	[searchShortDescription], 
	[searchRentalTerm], 
	[searchBuildingSize], 
	[searchBuildingUnit], 
	[searchERFSize], 
	[searchERFUnit],
	[searchImage], 
	[searchOnShow], 	
	[searchPropertySeeffSelect], 
	[searchPropertyFnbQuicksell], 
	[searchPropertyLastUpdated], 
	[searchAgentName], 
	[searchBranchName], 
	[searchPropertyGenieReference],  
	[searchPropertyGenieDateSuccess], 
	[searchPropertyPOA], 
	[searchPropertyESPServiceProvider], 
	[propCntrolMandateId], 
	[searchLongitude], 
	[searchLicensee], 
	[searchLatitude] 
)

--Do primary insert	
SELECT 
	 --get area hierarchy using scalar function
	 [dbo].[GetAreaPathMap](property.fkAreaId, 1),
	 [dbo].[GetAreaPathMap](property.fkAreaId, 2),
	 [dbo].[GetAreaPathMap](property.fkAreaId, 3),
	 [dbo].[GetAreaPathMap](property.fkAreaId, 4),
	 [dbo].[GetAreaPathMap](property.fkAreaId, 5),
	 [dbo].[GetAreaPathMap](property.fkAreaId, 6),
	 [seeff].[dbo].[area].[areaName],
	 [seeff].[dbo].[property].[propertyId],
	 [seeff].[dbo].[property].[fkAgentId],
	 [seeff].[dbo].[property].[fkBranchId],
	 [seeff].[dbo].[area].[areaParentId],
	 [seeff].[dbo].[property].[fkAreaId],
	 [seeff].[dbo].[property].[fkCategoryId],
	 [seeff].[dbo].[property].[fkPropertyTypeId],
	 [seeff].[dbo].[propertyType].[propertyTypeName],
	 [seeff].[dbo].[property].[fkActionId],
	 [seeff].[dbo].[property].[fkGolfEstateId],
	 [seeff].[dbo].[property].[propertyReference],
	 [seeff].[dbo].[property].[propertyPrice],
	 [seeff].[dbo].[property].[fkCurrencyId],
	 CAST([seeff].[dbo].[property].[propertyShortDescription] AS VARCHAR(max)),
	 [seeff].[dbo].[property].[propertyRentalTerm],
	 --Update the building size
		(CASE
			WHEN [seeff].[dbo].[property].[propertyBuildingSize] > 0	
				 
				THEN
					 CASE 
						WHEN [seeff].[dbo].[property].[propertyBuildingUnit] = 1 -- if it is m2 then do standard division
						AND [seeff].[dbo].[property].[propertyBuildingSize]  < 999999999  -- If the number is too large then we have an arithmetic overflow
						THEN [seeff].[dbo].[property].[propertyBuildingSize]

						WHEN [seeff].[dbo].[property].[propertyBuildingUnit] = 2 --If in hectares convert to 10000 m2
						AND [seeff].[dbo].[property].[propertyBuildingSize]  < 99999 -- If the number is too large then we have an arithmetic overflow
						THEN ([seeff].[dbo].[property].[propertyBuildingSize] * 10000)

						WHEN [seeff].[dbo].[property].[propertyBuildingUnit] = 3 --If in Km convert to = 1000000 m2
						AND [seeff].[dbo].[property].[propertyBuildingSize] < 999 -- If the number is too large then we have an arithmetic overflow
						THEN ([seeff].[dbo].[property].[propertyBuildingSize] * 1000000)

						ELSE 0
					  END

				ELSE 0
		  END
		),
	 [seeff].[dbo].[property].[propertyBuildingUnit],
	  --Update the erf size 
	  --use floor to deal with problem decimal values 
	  --address this in new seeff website
	  --CAST AS NUMERIC then do Math then CAST BACK TO VARCHAR
	(CASE 
			WHEN CAST(floor([seeff].[dbo].[property].[propertyERFSize]) AS NUMERIC) > 0 --<--CAST ERF size as Numeric from varchar for case check
				THEN
					 CASE 
						WHEN [seeff].[dbo].[property].[propertyERFUnit] = 1 -- if it is m2 then do standard division
						AND CAST(floor([seeff].[dbo].[property].[propertyERFSize]) AS NUMERIC)  < 999999999  -- If the number is too large then we have an arithmetic overflow
						THEN CAST(floor(CAST([seeff].[dbo].[property].[propertyERFSize] AS VARCHAR))AS NUMERIC)

						WHEN [seeff].[dbo].[property].[propertyERFUnit] = 2 --If in hectares convert to 10000 m2
						AND CAST(floor([seeff].[dbo].[property].[propertyERFSize]) AS NUMERIC)  < 99999 -- If the number is too large then we have an arithmetic overflow
						THEN CAST(floor((CAST([seeff].[dbo].[property].[propertyERFSize] AS VARCHAR) * 10000.00))AS NUMERIC)
				
						WHEN [seeff].[dbo].[property].[propertyERFUnit] = 3 --If in Km convert to = 1000000 m2
						AND CAST(floor([seeff].[dbo].[property].[propertyERFSize]) AS NUMERIC) < 999 -- If the number is too large then we have an arithmetic overflow
						THEN CAST(floor((CAST([seeff].[dbo].[property].[propertyERFSize] AS VARCHAR) * 1000000.00))AS NUMERIC)
						
						ELSE 0
					  END

				ELSE 0
		  END
		),
	 [seeff].[dbo].[property].[propertyERFUnit],
	 --Get the search image by selecting the default image
	--USE ORDER BY AND TOP 1 instead of a where clause for selecting the default image. Due to inconsistant data where there is no default image
	(SELECT TOP 1
		[seeff].[dbo].[property_images].[property_imagesName]
		FROM [seeff].[dbo].[property_images]
		WHERE [seeff].[dbo].[property_images].[propertyReference] = [seeff].[dbo].[property].[propertyReference]
		ORDER BY [seeff].[dbo].[property_images].[property_imagesDefault] DESC
	),
	 [seeff].[dbo].[property].[propertyOnShow],
	 [seeff].[dbo].[property].[propertySeeffSelect],
	 [seeff].[dbo].[property].[propertyFnbQuicksell],
	 [seeff].[dbo].[property].[propertyLastUpdated],
	 [seeff].[dbo].[agent].[agentFirstName] + ' ' + [seeff].[dbo].[agent].[agentSurname],
	 [seeff].[dbo].[branch].[branchName],
	 [seeff].[dbo].[property].[propertyGenieReference],
	 [seeff].[dbo].[property].[propertyGenieSuccessDate],
	 [seeff].[dbo].[property].[propertyPOA],
	 --Check for a ESP service provider
	 (CASE WHEN [seeff].[dbo].[property].[propertyESPServiceProvider] IS NOT NULL
		THEN [seeff].[dbo].[property].[propertyESPServiceProvider]
		WHEN [seeff].[dbo].[property].[propertyESPServiceProvider] IS NULL
		THEN 0
		--Else clause purely for data inconsistancies
		ELSE 0
	 END), 
	 [seeff].[dbo].[property].[propCntrolMandateId],
	 [seeff].[dbo].[property].[propertyLongitude],
	 [seeff].[dbo].[property].[propertyLicensee],
	 [seeff].[dbo].[property].[propertyLatitude]

--Do table join
FROM [seeff].[dbo].[property]
INNER JOIN
[seeff].[dbo].[agent]
ON [seeff].[dbo].[property].[fkAgentId]
= [seeff].[dbo].[agent].[agentId]
INNER JOIN 
[seeff].[dbo].[branch]
ON
[seeff].[dbo].[property].[fkBranchId]
= [seeff].[dbo].[branch].[branchId]
INNER JOIN
[seeff].[dbo].[area]
ON
[seeff].[dbo].[property].[fkAreaId]
=[seeff].[dbo].[area].[areaId]
INNER JOIN
[seeff].[dbo].[propertyType]
ON
[seeff].[dbo].[property].[fkPropertyTypeId]
=[seeff].[dbo].[propertyType].[propertyTypeId]

--Set Condition for property on search table, Stops incomplete datasets from being populated
WHERE [seeff].[dbo].[property].[propertyActive] = 1
AND [seeff].[dbo].[property].[fkAreaId] > 0
AND [seeff].[dbo].[property].[fkAgentId] > 0
AND [seeff].[dbo].[property].[propertyReference] = @webRef




-- Do secondary Updates for values which could not be accomodated in primary insert

-- Get the parent area Name 
UPDATE  [seeff].[dbo].[search]
	SET	[seeff].[dbo].[search].[searchAreaParentName] 
	= (SELECT [dbo].[area].[areaName]
	  FROM [dbo].[area]
	  WHERE [dbo].[area].[areaId] = [dbo].[search].[fkAreaParentId])
	  FROM  search WHERE [seeff].[dbo].[search].[searchReference] = @webRef

-- Get the Adjusted price, this is only for daily rentals, defaults to normal price 
UPDATE  [seeff].[dbo].[search]
	 SET [searchPriceAdjusted] = 
	 (CASE 
		WHEN[seeff].[dbo].[search].[fkActionId] = 3
			THEN
				CASE
					WHEN [searchRentalTerm] = 'daily' --if daily rentail then set adjusted price
					THEN FLOOR(([seeff].[dbo].[search].[searchPrice] * 365) / 12)
					WHEN [searchRentalTerm] = 'monthly'
					THEN [seeff].[dbo].[search].[searchPrice]
					WHEN [searchRentalTerm] = 'n/a'
					THEN [seeff].[dbo].[search].[searchPrice]
			END
		WHEN [seeff].[dbo].[search].[fkActionId] = 2
		THEN [seeff].[dbo].[search].[searchPrice]
		--Else clause purely for data inconsistancies
		ELSE [seeff].[dbo].[search].[searchPrice]
	  END
	)FROM search WHERE [seeff].[dbo].[search].[searchReference] = @webRef



--Caculate building per square meter cost
UPDATE [seeff].[dbo].[search]
SET searchBuildingPerm2 =
	(CASE
		WHEN [seeff].[dbo].[search].[searchBuildingSize] > 0	
			THEN round(([seeff].[dbo].[search].[searchPriceAdjusted]) / (CAST([seeff].[dbo].[search].[searchBuildingSize] AS MONEY)), 2)
			ELSE 0.00
	  END
	) FROM search WHERE [seeff].[dbo].[search].[searchReference] = @webRef

--Caculate land per square meter cost
UPDATE [seeff].[dbo].[search]
SET searchLandPerm2 =
	(CASE
		WHEN CAST([seeff].[dbo].[search].[searchERFSize] AS NUMERIC) > 0 --<--cast as numeric in order to deal with the varchar column
			THEN round(([seeff].[dbo].[search].[searchPriceAdjusted]) / (CAST([seeff].[dbo].[search].[searchERFSize] AS MONEY)), 2)
			ELSE 0.00
	  END
) FROM search WHERE [seeff].[dbo].[search].[searchReference] = @webRef


--Get Features
--Get the bedrooms
UPDATE [seeff].[dbo].[search]
	SET [searchBedrooms] = replace([seeff].[dbo].[property_feature].[property_featureValue], '+', '') --Replace + symbol as [searchBedrooms] is integer 
	FROM [seeff].[dbo].[search]
	INNER JOIN [seeff].[dbo].[property_feature]
	ON [seeff].[dbo].[search].[fkPropertyId] = [seeff].[dbo].[property_feature].[fkPropertyId]
	WHERE [seeff].[dbo].[search].[fkPropertyId] = [seeff].[dbo].[property_feature].[fkPropertyId]
	AND [seeff].[dbo].[property_feature].[fkfeatureId] = 1
	AND [seeff].[dbo].[search].[searchReference] = @webRef

--Get the bathrooms
UPDATE [seeff].[dbo].[search]
SET	[searchBathrooms] = replace([seeff].[dbo].[property_feature].[property_featureValue] , '+', '') --Replace + symbol as [searchBathrooms] is integer 
	FROM [seeff].[dbo].[search]
	INNER JOIN [seeff].[dbo].[property_feature]
	ON [seeff].[dbo].[search].[fkPropertyId] = [seeff].[dbo].[property_feature].[fkPropertyId]
	WHERE [seeff].[dbo].[search].[fkPropertyId] = [seeff].[dbo].[property_feature].[fkPropertyId]
	AND [seeff].[dbo].[property_feature].[fkfeatureId] = 2
	AND [seeff].[dbo].[search].[searchReference] = @webRef
--Get the garages
UPDATE [seeff].[dbo].[search]
SET	[searchGarages] = replace([seeff].[dbo].[property_feature].[property_featureValue], '+', '') --Replace + symbol as [searchGarages] is integer 
	FROM [seeff].[dbo].[search]
	INNER JOIN [seeff].[dbo].[property_feature]
	ON [seeff].[dbo].[search].[fkPropertyId] = [seeff].[dbo].[property_feature].[fkPropertyId]
	WHERE [seeff].[dbo].[search].[fkPropertyId] = [seeff].[dbo].[property_feature].[fkPropertyId]
	AND [seeff].[dbo].[property_feature].[fkfeatureId] = 3
	AND [seeff].[dbo].[search].[searchReference] = @webRef
--Get the Secure Parking
UPDATE [seeff].[dbo].[search]
SET	[searchSecureParking] = replace([seeff].[dbo].[property_feature].[property_featureValue] , '+', '') --Replace + symbol as [searchSecureParking] is integer 
	FROM [seeff].[dbo].[search]
	INNER JOIN [seeff].[dbo].[property_feature]
	ON [seeff].[dbo].[search].[fkPropertyId] = [seeff].[dbo].[property_feature].[fkPropertyId]
	WHERE [seeff].[dbo].[search].[fkPropertyId] = [seeff].[dbo].[property_feature].[fkPropertyId]
	AND [seeff].[dbo].[property_feature].[fkfeatureId] = 4
	AND [seeff].[dbo].[search].[searchReference] = @webRef
--Get the bedrooms
UPDATE [seeff].[dbo].[search]
SET [searchReceptions]  = replace([seeff].[dbo].[property_feature].[property_featureValue] , '+', '') --Replace + symbol as [searchReceptions] is integer 
	FROM [seeff].[dbo].[search]
	INNER JOIN [seeff].[dbo].[property_feature]
	ON [seeff].[dbo].[search].[fkPropertyId] = [seeff].[dbo].[property_feature].[fkPropertyId]
	WHERE [seeff].[dbo].[search].[fkPropertyId] = [seeff].[dbo].[property_feature].[fkPropertyId]
	AND [seeff].[dbo].[property_feature].[fkfeatureId] = 5
	AND [seeff].[dbo].[search].[searchReference] = @webRef

--Check if the property is sold by checking if the sold date is bigger than the default date
UPDATE [seeff].[dbo].[search]
SET [seeff].[dbo].[search].[searchPropertySold] =
	(CASE
		--Only if there is a sold date and selling price is the property marked as sold
		WHEN [seeff].[dbo].[property].[fkSellerId] > 0
		THEN 1
		ELSE 0
	END)
	FROM [seeff].[dbo].[search] INNER JOIN
	[seeff].[dbo].[property]
	ON [seeff].[dbo].[search].[searchReference] = 
	[seeff].[dbo].[property].[propertyReference]
	WHERE [seeff].[dbo].[search].[searchReference] = @webRef


--correct the show on map coordinates
UPDATE [dbo].[map_property]
   SET --[map_property_latitude] = CAST(left([propertyLatitude], 11) AS VARCHAR)
      --,[map_property_longitude] = CAST(left([propertyLongitude], 10) AS VARCHAR)
	  [map_property_latitude] = CAST([propertyLatitude] AS VARCHAR(11))
      ,[map_property_longitude] = CAST([propertyLongitude] AS VARCHAR(10))
	  FROM [map_property] JOIN property
	  ON [map_property].fk_property_id = property.propertyId
 WHERE [fk_property_id] = propertyId
 AND property.propertyActive = 1
 AND [seeff].[dbo].[property].propertyReference = @webRef
	
END

