
CREATE VIEW dbo.propertyStadiumView
AS
SELECT     dbo.property_stadium.fkPropertyId, dbo.property_stadium.property_stadium_distance, dbo.stadium_2010.pk_stadium_2010_id, 
                      dbo.stadium_2010.stadium_2010_name, dbo.stadium_2010.stadium_2010_page, dbo.city_2010.pk_city_2010_id, dbo.city_2010.city_2010_name, 
                      dbo.city_2010.city_2010_page
FROM         dbo.property_stadium INNER JOIN
                      dbo.stadium_2010 ON dbo.property_stadium.fk_stadium_2010_id = dbo.stadium_2010.pk_stadium_2010_id INNER JOIN
                      dbo.city_2010 ON dbo.stadium_2010.fk_city_2010_id = dbo.city_2010.pk_city_2010_id

