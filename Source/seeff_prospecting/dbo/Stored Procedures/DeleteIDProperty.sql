

-- =============================================
-- Author:		Adam Roberts
-- Create date: 2014-11-08
-- Description:	Delete a Property that failed to load properly the first time 
-- =============================================
CREATE PROCEDURE [dbo].[DeleteIDProperty]
 @prospecting_property_id int 
AS
BEGIN
PRINT @prospecting_property_id
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

delete  p from [dbo].[prospecting_person_property_relationship] p
join  [dbo].[prospecting_property] pp on pp.prospecting_property_id = p.prospecting_property_id
where pp.prospecting_property_id = @prospecting_property_id

delete p from [dbo].[prospecting_trace_ps_enquiry] p
join [dbo].[prospecting_property] pp
on pp.prospecting_property_id = p.prospecting_property_id
where pp.prospecting_property_id = @prospecting_property_id


delete c from [dbo].[prospecting_company_property_relationship] c
join [prospecting_property] pp on pp.prospecting_property_id = c.prospecting_property_id
where pp.prospecting_property_id = @prospecting_property_id

delete from [prospecting_property]
where prospecting_property_id = @prospecting_property_id

END


