
-- =============================================
-- Author:		Scott Murray
-- Create date: 2013.06.03
-- Description:	This Proc builds the IOL Feed
--              column as well as the seeff Feed
--				column
-- =============================================
CREATE PROCEDURE [dbo].[sp_feed_out_service_build]
	-- Add the parameters for the stored procedure here
	 @timeFrame DATETIME = NULL,
	 @startTime DATETIME = NULL,
	 @endTime DATETIME = NULL
	 --,@get_images varchar(max)
AS
--SET the variables
SET @timeFrame = DATEADD(mm, -12, Getdate ())
SET @startTime = GETDATE()

--if the iol table object exists drop it
--dont use 'if exists' with select statement
--we dont know if the data set is empty

IF OBJECT_ID('feed_out_service', 'U') IS NOT NULL
	DROP TABLE [seeff].[dbo].[feed_out_service]

--Create the iol table
	CREATE TABLE [seeff].[dbo].[feed_out_service]
	(
	[feed_id] [int] IDENTITY(1,1) NOT NULL Primary Key,
	[province] VARCHAR (20) NOT NULL,
	[seeff_web_ref] INT NOT  NULL,
	[branch_id] INT NOT NULL,
	[branch_name] VARCHAR (max) NOT NULL,
	[seeff_feed] VARCHAR (max)  NULL,
	[iol_feed] VARCHAR (max)  NULL
	)	

BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;



	INSERT INTO [seeff].[dbo].[feed_out_service]
	([province],
	[seeff_web_ref],
	[branch_id],
	[branch_name],
    [seeff_feed],
	[iol_feed])
	
	SELECT 
	       CASE 
				WHEN [seeff].[dbo].[search].[searchProvince] = 2 THEN 'Western Cape'
				WHEN [seeff].[dbo].[search].[searchProvince] = 11 THEN 'Gauteng'
				WHEN [seeff].[dbo].[search].[searchProvince] = 12 THEN 'Free State'
				WHEN [seeff].[dbo].[search].[searchProvince] = 13 THEN 'Kwazulu Natal'
				WHEN [seeff].[dbo].[search].[searchProvince] = 14 THEN 'Limpopo'
				WHEN [seeff].[dbo].[search].[searchProvince] = 15 THEN 'Mpumalanga'
				WHEN [seeff].[dbo].[search].[searchProvince] = 16 THEN 'North West Province'
				WHEN [seeff].[dbo].[search].[searchProvince] = 17 THEN 'Eastern Cape'
				WHEN [seeff].[dbo].[search].[searchProvince] = 18 THEN 'Northern Cape' 
				ELSE ''
			END 
		   ,[seeff].[dbo].[search].[searchReference]
		   ,[seeff].[dbo].[search].[fkBranchId]
		   ,[seeff].[dbo].[branch].[branchName]
		   ,
		   /*********************************BEGIN SEEFF XML FEED INSERT*************************************/
	    	'<listing>' 
		 + '<seeffPropertyReference>' +  CAST([seeff].[dbo].[property].[propertyReference] AS VARCHAR) +  '</seeffPropertyReference>' 
		 +'<seeffPropertyLink>http://www.seeff.com/'
		 +(CASE 
				WHEN [seeff].[dbo].[search].[fkActionId] = 2 THEN 'buy/'
				WHEN [seeff].[dbo].[search].[fkActionId] = 3 THEN 'rent/'
				ELSE 'buy/'
            END)
		 +(CASE
				WHEN [seeff].[dbo].[search].[fkCategoryId] = 1 THEN 'residential/'
				WHEN [seeff].[dbo].[search].[fkCategoryId] = 2 THEN 'commercial/'
				WHEN [seeff].[dbo].[search].[fkCategoryId] = 5 THEN 'agricultural/'
				ELSE 'residential/'
		   END)
		   + 'details.php?pref='
		   + CAST([seeff].[dbo].[property].[propertyReference] AS VARCHAR)
		   + '&&trafficMonitor='
		   +'</seeffPropertyLink>'
		   + '<branch>' 
				+  '<branchId>' + CAST([seeff].[dbo].[search].[fkBranchId] AS VARCHAR) + '</branchId>'
				+  '<branchName>' + [seeff].[dbo].[branch].[branchName] + '</branchName>'
			--	+  '<latitude>' + +'</latitude>' -- for later release
			--	+  '<longitude>' + + '</longitude>' -- for later release
		   + '</branch>'
		   + '<property>'
				+ '<location>'
					+ '<province>' + (SELECT [seeff].[dbo].[area].[areaName] FROM [seeff].[dbo].[area] WHERE [seeff].[dbo].[area].[areaId] = [seeff].[dbo].[search].[searchProvince])+ '</province>'
					+ '<area>' + REPLACE([seeff].[dbo].[search].[searchAreaName], '&', '&amp;') + ', ' +[seeff].[dbo].[search].[searchAreaParentName] + '</area>'
				+ '</location>'
				+ '<propertyType>' +  [seeff].[dbo].[search].[searchPropertyTypeName] + '</propertyType>' --house/villa etc
				+ '<status>' +(CASE 
								WHEN [seeff].[dbo].[search].[fkActionId] = 2 THEN 'For Sale'
								WHEN [seeff].[dbo].[search].[fkActionId] = 3 THEN 'To Let'
							  END) 
				+'</status>'--for sale/ to rent
				
					+ (CASE 
								WHEN [seeff].[dbo].[search].[fkActionId] = 3 
									THEN 
										CASE 
											WHEN [seeff].[dbo].[search].[searchRentalTerm] = 'monthly' 
											THEN '<rentalTerm>monthly</rentalTerm>'
											WHEN [seeff].[dbo].[search].[searchRentalTerm] = 'daily' 
											THEN '<rentalTerm>daily</rentalTerm>'
											ELSE '<rentalTerm>n/a</rentalTerm>'
										END
								ELSE '<rentalTerm>n/a</rentalTerm>' 
						  END) 
				+ '<price>' + CAST (FLOOR([seeff].[dbo].[search].[searchPrice]) AS varchar) + '</price>' 
				+ '<feature>' 
				+	(CASE WHEN [seeff].[dbo].[search].[searchBedrooms] > 0
					 THEN
						+ '<bedroom>' 
							+ '<quantity>' + CAST ([seeff].[dbo].[search].[searchBedrooms] AS varchar) + '</quantity>' 
						+ '</bedroom>' 
					ELSE +  '<bedroom><quantity>0</quantity></bedroom>'
					END)
				+	(CASE WHEN [seeff].[dbo].[search].[searchBathrooms] > 0
					 THEN
					+ '<bath>' 
						+ '<quantity>' + CAST ([seeff].[dbo].[search].[searchBathrooms] AS varchar) + '</quantity>' 
					+ '</bath>' 
					ELSE +  '<bath><quantity>0</quantity></bath>'
					END)
				+	(CASE WHEN [seeff].[dbo].[search].[searchGarages] > 0
					 THEN
					+ '<garage>' 
						+ '<quantity>' + CAST ([seeff].[dbo].[search].[searchGarages] AS varchar) + '</quantity>' 
					+ '</garage>' 
					ELSE +  '<garage><quantity>0</quantity></garage>'
					END)
				+	(CASE WHEN [seeff].[dbo].[search].[searchSecureParking] > 0
					 THEN
					+ '<secureParking>' 
						+ '<quantity>' + CAST ([seeff].[dbo].[search].[searchSecureParking] AS varchar) + '</quantity>' 
					+ '</secureParking>' 
					ELSE +  '<secureParking><quantity>0</quantity></secureParking>'
					END)
				+	(CASE WHEN [seeff].[dbo].[search].[searchReceptions] > 0
					 THEN
					+ '<reception>' 
						+ '<quantity>' + CAST ([seeff].[dbo].[search].[searchReceptions] AS varchar) + '</quantity>' 
					+ '</reception>	' 
					ELSE +   '<reception><quantity>0</quantity></reception>'
					END)
				+ '</feature>' 
				+ '<propertyImage>' 
					+ (SELECT TOP 1
							CASE 
								WHEN [seeff].[dbo].[search].[propCntrolMandateId] IS NOT NULL THEN 'http://newimages1.seeff.com/images/'
								+ CAST ([seeff].[dbo].[property_images].[propertyReference] AS varchar) + '/'
								+ CAST ([seeff].[dbo].[property_images].[property_imagesName] AS varchar) 
								WHEN [seeff].[dbo].[search].[propCntrolMandateId] IS NULL THEN ':http://images1.seeff.com/images/property/'
								+ CAST ([seeff].[dbo].[property_images].[propertyReference] AS varchar) + '/lg_'
								+ CAST ([seeff].[dbo].[property_images].[property_imagesName] AS varchar)  
							 END
						FROM [seeff].[dbo].[property_images]
						WHERE [seeff].[dbo].[property_images].[propertyReference] = [seeff].[dbo].[search].[searchReference]
						ORDER BY [seeff].[dbo].[property_images].[property_imagesDefault] DESC
						)
				+ '</propertyImage>' 
			+  '<propertyShortDescription>'  + REPLACE(REPLACE(REPLACE(CAST([seeff].[dbo].[search].[searchShortDescription] AS VARCHAR(1000)), '&', '&amp;'),'<','&lt;'),'>','&gt;')  + '</propertyShortDescription>' 
				+ '<propertyLongDescription>'  + REPLACE(REPLACE(REPLACE(CAST([seeff].[dbo].[property].[propertyFullDescription] AS VARCHAR(2000)), '&', '&amp;'),'<','&lt;'),'>','&gt;')  + '</propertyLongDescription>' 
					 
				+(CASE WHEN CAST([seeff].[dbo].[search].[searchERFSize] AS VARCHAR)  != '0'
						  THEN
								+ '<erf>' 
								+ '<measurementUnit>' + (CASE 
														WHEN [seeff].[dbo].[search].[searchERFUnit] = 1
														THEN 'm²'
														WHEN [seeff].[dbo].[search].[searchERFUnit] = 2
														THEN 'hectares'
														WHEN [seeff].[dbo].[search].[searchERFUnit] = 3
														THEN 'km²'
														ELSE 'm²'
														END)
								+ '</measurementUnit>' 
								+ '<size>' + CAST([seeff].[dbo].[search].[searchERFSize] AS VARCHAR) + '</size>' 
								+ '</erf>' 
						  ELSE + '<erf><measurementUnit>n/a</measurementUnit><size>n/a</size></erf>'
				 END)
				
				+(CASE WHEN CAST([seeff].[dbo].[search].[searchBuildingSize]AS VARCHAR)  != '0' 
						   THEN
							+ '<building>' 
							+ '<measurementUnit>' + (CASE 
													WHEN [seeff].[dbo].[search].[searchERFUnit] = 1
													THEN 'm²'
													WHEN [seeff].[dbo].[search].[searchERFUnit] = 2
													THEN 'hectares'
													WHEN [seeff].[dbo].[search].[searchERFUnit] = 3
													THEN 'km²'
													ELSE 'm²'
													END)
							+ '</measurementUnit>' 
							+ '<size>' + CAST([seeff].[dbo].[search].[searchBuildingSize] AS VARCHAR) + '</size>' 
						+ '</building>' 
							  ELSE + '<building><measurementUnit>n/a</measurementUnit><size>n/a</size></building>'
				 END) 
				+ '<agentMandateType>' + RTRIM([seeff].[dbo].[property].[propertyMandate]) + '</agentMandateType>' --sole/open/joint
				+ '<lastUpdated>' + CONVERT (varchar, FORMAT([seeff].[dbo].[search].[searchPropertyLastUpdated], 'yyyy-MM-dd HH:mm:ss')) + '</lastUpdated>' --date property was last updated
		   + '</property>'  
		   + '</listing>' 
		 ,
		 /*********************************BEGIN IOL FEED INSERT*************************************/
		 	'[[Listing_Start]]' + CHAR(10)
		 +  '[[Province:'
		 + (CASE 
				WHEN [seeff].[dbo].[search].[searchProvince] = 2 THEN 'Western Cape'
				WHEN [seeff].[dbo].[search].[searchProvince] = 11 THEN 'Gauteng'
				WHEN [seeff].[dbo].[search].[searchProvince] = 12 THEN 'Free State'
				WHEN [seeff].[dbo].[search].[searchProvince] = 13 THEN 'Kwazulu Natal'
				WHEN [seeff].[dbo].[search].[searchProvince] = 14 THEN 'Limpopo'
				WHEN [seeff].[dbo].[search].[searchProvince] = 15 THEN 'Mpumalanga'
				WHEN [seeff].[dbo].[search].[searchProvince] = 16 THEN 'North West Province'
				WHEN [seeff].[dbo].[search].[searchProvince] = 17 THEN 'Eastern Cape'
				WHEN [seeff].[dbo].[search].[searchProvince] = 18 THEN 'Northern Cape'
				ELSE '' 
			END)
		 +  '/]]' + CHAR(10)
		 +  '[[Area:' + CAST ([seeff].[dbo].[search].[searchAreaName] AS varchar) + '/]]' + CHAR(10)
		 +  '[[Reference:' + CAST ([seeff].[dbo].[search].[searchReference] AS varchar) + '/]]' + CHAR(10)
		 +  '[[Status:' 
		 +  (CASE 
				WHEN [seeff].[dbo].[search].[fkActionId] = 2 THEN 'For Sale'
				WHEN [seeff].[dbo].[search].[fkActionId] = 3 THEN 'To Let'
            END)
		 +  '/]]' + CHAR(10)
		 +  '[[Price:' + CAST (FLOOR([seeff].[dbo].[search].[searchPrice]) AS varchar) + '/]]' + CHAR(10)
		 +  '[[Type:' 
		 + (CASE
			        WHEN [seeff].[dbo].[search].[searchPropertyTypeName]  = 'Apartment' THEN  'Apartment'
                    WHEN [seeff].[dbo].[search].[searchPropertyTypeName]  = 'Aquaculture' THEN 'Agricultural'
                    WHEN [seeff].[dbo].[search].[searchPropertyTypeName]  = 'Businesses' THEN 'Commercial'
                    WHEN [seeff].[dbo].[search].[searchPropertyTypeName]  = 'Cluster' THEN 'House'
                    WHEN [seeff].[dbo].[search].[searchPropertyTypeName]  = 'Commercial Land'THEN 'Commercial'
                    WHEN [seeff].[dbo].[search].[searchPropertyTypeName]  = 'Crop / Livestock / Dairy / Stud' THEN 'Agricultural'
                    WHEN [seeff].[dbo].[search].[searchPropertyTypeName]  = 'Flowers / Vegetables' THEN 'Agricultural'
                    WHEN [seeff].[dbo].[search].[searchPropertyTypeName]  = 'Garden Cottage' THEN 'House'
                    WHEN [seeff].[dbo].[search].[searchPropertyTypeName]  = 'Golf Estate' THEN 'House'
                    WHEN [seeff].[dbo].[search].[searchPropertyTypeName]  = 'Guesthouse' THEN 'Guest House'
                    WHEN [seeff].[dbo].[search].[searchPropertyTypeName]  = 'Holiday' THEN 'Other'
                    WHEN [seeff].[dbo].[search].[searchPropertyTypeName]  = 'Holiday Apartment' THEN 'Apartment'
                    WHEN [seeff].[dbo].[search].[searchPropertyTypeName]  = 'Holiday House' THEN 'House'
                    WHEN [seeff].[dbo].[search].[searchPropertyTypeName]  = 'Holiday Resort' THEN 'Other'
                    WHEN [seeff].[dbo].[search].[searchPropertyTypeName]  = 'Hotel / Tourism' THEN 'Commercial'
                    WHEN [seeff].[dbo].[search].[searchPropertyTypeName]  = 'House/Villa' THEN 'House'
                    WHEN [seeff].[dbo].[search].[searchPropertyTypeName]  = 'Industrial Factories' THEN 'Commercial'
                    WHEN [seeff].[dbo].[search].[searchPropertyTypeName]  = 'Industrial Land' THEN 'Commercial'
                    WHEN [seeff].[dbo].[search].[searchPropertyTypeName]  = 'Investment Properties' THEN 'Other'
                    WHEN [seeff].[dbo].[search].[searchPropertyTypeName]  = 'Lifestyle / Small Holding' THEN 'Smallholding'
                    WHEN [seeff].[dbo].[search].[searchPropertyTypeName]  = 'Lifestyle Estates' THEN 'Complex'
                    WHEN [seeff].[dbo].[search].[searchPropertyTypeName]  = 'Lodge' THEN 'Commercial'
                    WHEN [seeff].[dbo].[search].[searchPropertyTypeName]  = 'Nature / Game Reserve' THEN 'Agricultural'
                    WHEN [seeff].[dbo].[search].[searchPropertyTypeName]  = 'Offices'THEN 'Commercial'
                    WHEN [seeff].[dbo].[search].[searchPropertyTypeName]  = 'Organic'THEN 'Agricultural'
                    WHEN [seeff].[dbo].[search].[searchPropertyTypeName]  = 'Rentals'THEN 'House'
                    WHEN [seeff].[dbo].[search].[searchPropertyTypeName]  = 'Retail / Shops' THEN'Commercial'
                    WHEN [seeff].[dbo].[search].[searchPropertyTypeName]  = 'Retirement Village' THEN 'Complex'
                    WHEN [seeff].[dbo].[search].[searchPropertyTypeName]  = 'Riverfrontage' THEN 'House'
                    WHEN [seeff].[dbo].[search].[searchPropertyTypeName]  = 'Security Village' THEN 'Complex'
                    WHEN [seeff].[dbo].[search].[searchPropertyTypeName]  = 'Townhouse' THEN'Complex'
                    WHEN [seeff].[dbo].[search].[searchPropertyTypeName]  = 'Vacant Land' THEN 'Vacant Land'
                    WHEN [seeff].[dbo].[search].[searchPropertyTypeName]  = 'Wine / Fruit' THEN 'Agricultural'
                    ELSE 'Other'
			END) 
		 + '/]]' + CHAR(10)
		 +  '[[Beds:' + 
			(CASE WHEN [seeff].[dbo].[search].[searchBedrooms] > 0
					 THEN  CAST ([seeff].[dbo].[search].[searchBedrooms] AS varchar) 
					 ELSE '0'
			END)
		 + '/]]' + CHAR(10)
		 +  '[[Baths:' + (CASE WHEN [seeff].[dbo].[search].[searchBathrooms] > 0
					 THEN  CAST ([seeff].[dbo].[search].[searchBathrooms] AS varchar) 
					 ELSE '0'
			END)
		 + '/]]'  + CHAR(10)
         +  '[[Garages:' + (CASE WHEN [seeff].[dbo].[search].[searchGarages] > 0
							THEN CAST ([seeff].[dbo].[search].[searchGarages] AS varchar) 
							ELSE '0'
							END)
		 + '/]]' + CHAR(10)
         +  '[[Carports:' + (CASE WHEN [seeff].[dbo].[search].[searchSecureParking] > 0
							THEN CAST ([seeff].[dbo].[search].[searchSecureParking] AS varchar) 
							ELSE '0'
			END)
		 + '/]]' + CHAR(10)
         +  '[[Living_Rooms:' + (CASE WHEN [seeff].[dbo].[search].[searchReceptions] > 0
							THEN CAST ([seeff].[dbo].[search].[searchReceptions] AS varchar)
							ELSE '0'
			END) 
		 + '/]]' + CHAR(10)
		 +  '[[Heading:'+  REPLACE(CAST([seeff].[dbo].[property].[propertyShortDescription] AS VARCHAR(500)), '’' , '''') + '/]]' + CHAR(10)
         +  '[[Description:' +  LTRIM(RTRIM(REPLACE(CAST([seeff].[dbo].[property].[propertyFullDescription]  AS VARCHAR(5000)), '’' , ''''))) + '/]]' + CHAR(10)	

		 /*********START handle images*******/
		 -- Group by Data using column and XML PATH
		 + (CASE 
		   WHEN [seeff].[dbo].[property].[propCntrolMandateId] IS NOT NULL  THEN
			(SELECT DISTINCT
			STUFF((
			SELECT '' + '[[Image_URL:http://newimages1.seeff.com/images/' 
						+ CAST ([seeff].[dbo].[property_images].[propertyReference] AS varchar) 
						+ '/'
						+ CAST ([seeff].[dbo].[property_images].[property_imagesName] AS varchar(80)) 
						+ '/]]'	+ CHAR(10)
			FROM property_images
			WHERE (property_images.propertyReference = property.propertyReference)
			FOR XML PATH (''))
			,1,2,'[[') AS NameValues
			FROM property_images 
			WHERE property_images.propertyReference = property.propertyReference
			GROUP BY property_images.propertyReference
			)
		WHEN [seeff].[dbo].[property].[propCntrolMandateId] IS NULL  THEN
			(SELECT DISTINCT
			STUFF((
			SELECT '' + '[[Image_URL:http://images1.seeff.com/images/property/' 
						+ CAST ([seeff].[dbo].[property_images].[propertyReference] AS varchar) 
						+ '/lg_'
						+ CAST ([seeff].[dbo].[property_images].[property_imagesName] AS varchar) 
						+ '/]]'	+ CHAR(10)
			FROM property_images
			WHERE (property_images.propertyReference = property.propertyReference)
			FOR XML PATH (''))
			,1,2,'[[') AS NameValues
			FROM property_images 
			WHERE property_images.propertyReference = property.propertyReference
			GROUP BY property_images.propertyReference
			)

		END)
		
		/**********END handle images*******/
		 +  '[[Details_URL:http://www.seeff.com/' + REPLACE(LOWER([action].[actionName]), ' ', '') +'/' + REPLACE(REPLACE(REPLACE(REPLACE(LOWER([category].[categoryName]),'Developments','residential'),'/ industrial', ''),'property', ''), ' ', '') +'/details.php?pref=' + CAST ([seeff].[dbo].[search].[searchReference] AS varchar) + '/]]'+ CHAR(10)
		 +	'[[Agent_Name:' + [seeff].[dbo].[agent].[agentFirstName] +  ' '  + [seeff].[dbo].[agent].[agentSurname] + '/]]'+ CHAR(10)
		 +  '[[Office_No:' + CAST (RTRIM(REPLACE(REPLACE(REPLACE(REPLACE([seeff].[dbo].[agent].[agentTelephone], ' ', ''), '-', ''), '(', ''), ')' ,'')) AS varchar) + '/]]'+ CHAR(10) --Revisit numbers as integers
	     +  '[[Cell_No:' + CAST (RTRIM(REPLACE(REPLACE(REPLACE(REPLACE([seeff].[dbo].[agent].[agentCell], ' ', ''), '-', ''), '(', ''), ')' ,'')) AS varchar) + '/]]'+ CHAR(10) --Revisit numbers as integers
		 +	'[[Email:' + [seeff].[dbo].[agent].[agentEmail] + '/]]'+ CHAR(10)
		 +	'[[Branch_Name:' + [seeff].[dbo].[branch].[branchName] + '/]]'+ CHAR(10)
		 +	'[[Branch_ID:' + CAST ([seeff].[dbo].[branch].[branchId] AS varchar) + '/]]'+ CHAR(10)			 
			+(CASE WHEN CAST([seeff].[dbo].[search].[searchERFSize] AS VARCHAR)  > '0'
						  THEN '[[Erf_Size:' + CAST ([seeff].[dbo].[property].[propertyERFSize] AS varchar) + '/]]'+ CHAR(10)
						  ELSE '[[Erf_Size:/]]'+ CHAR(10)
			 END)
		 +	'[[Building_Size:' + CAST ([seeff].[dbo].[property].[propertyBuildingSize] AS varchar) + '/]]'+ CHAR(10)
		 +	'[[Listed:' + CONVERT (varchar, FORMAT([seeff].[dbo].[property].[propertyCreated], 'yyyy-MM-dd HH:mm:ss')) + '/]]'+ CHAR(10)
	     +  '[[Listing_End]]' + CHAR(10)
		 /******************************************END INSERTS*************************************************/
	FROM 	[seeff].[dbo].[search] 
			INNER JOIN
			[seeff].[dbo].property 
			ON [seeff].[dbo].[search].[searchReference] = [seeff].[dbo].[property].[propertyReference]  
			INNER JOIN
			[seeff].[dbo].[agent] 
			ON [seeff].[dbo].[search].[fkAgentId] = [seeff].[dbo].[agent].[agentId]
			INNER JOIN
			[seeff].[dbo].[branch] 
			ON [seeff].[dbo].[search].[fkBranchId] = [seeff].[dbo].[branch].[branchId]
			INNER JOIN 
			[seeff].[dbo].[category]
			ON [seeff].[dbo].[search].fkCategoryId = [seeff].[dbo].[category].[categoryId]
			INNER JOIN 
			[seeff].[dbo].[action]
			ON [seeff].[dbo].[search].[fkActionId] = [seeff].[dbo].[action].[actionId]
			WHERE [seeff].[dbo].[search].[searchPropertyLastUpdated] > @timeFrame
			AND   [seeff].[dbo].[search].[searchProvince] NOT IN (-1, 0, 2818)
			AND   [seeff].[dbo].[search].[searchProvince] IS NOT NULL
			AND   [seeff].[dbo].[search].[searchImage] IS NOT NULL
			AND   [seeff].[dbo].[search].[searchImage] != ''
			AND   [seeff].[dbo].[property].[propertyActive] = 1
			AND   [seeff].[dbo].[property].[fkSellerId] = 0
			AND	  [seeff].[dbo].[search].[fkAgentId] != 0
			AND	  [seeff].[dbo].[search].[fkAgentId] IS NOT NULL
			AND	  [seeff].[dbo].[search].[fkAgentId] != ''
			AND	  [seeff].[dbo].[search].[fkBranchId] != 285--Added 24/11/2014

	ORDER BY [seeff].[dbo].[search].[fkBranchId] 
	
	/*SET THE END TIM WHEN QUERY FINISHES EXECUTING*/
	SET @endTime = GETDATE()

	/*Print start and end time for logging*/
	print 'START TIME: ' + CAST(@startTime AS VARCHAR)
	print 'END TIME: ' + CAST(@endTime AS VARCHAR)
	print 'TOTAL EXECUTION TIME IN SECONDS: ' +  CAST(DATEDIFF(ss,@startTime,@endTime) AS VARCHAR)
	 

END

