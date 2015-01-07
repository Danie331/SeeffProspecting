CREATE VIEW dbo.stadiumView
AS
SELECT     dbo.stadium_2010.pk_stadium_2010_id, dbo.stadium_2010.stadium_2010_name, dbo.city_2010.city_2010_name, 
                      dbo.stadium_2010.fk_city_2010_id
FROM         dbo.stadium_2010 INNER JOIN
                      dbo.city_2010 ON dbo.stadium_2010.fk_city_2010_id = dbo.city_2010.pk_city_2010_id
