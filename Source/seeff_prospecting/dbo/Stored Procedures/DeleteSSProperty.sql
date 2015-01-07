-- =============================================
-- Author:		Adam Roberts
-- Create date: 2014-11-08
-- Description:	Delete a sectional title that failed to loed properly the first time
-- =============================================
CREATE PROCEDURE DeleteSSProperty
 @street_no varchar(max),
 @prop_address varchar(max),
 @ss_name varchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	delete  p from [dbo].[prospecting_person_property_relationship] p
join  [dbo].[prospecting_property] pp on pp.prospecting_property_id = p.prospecting_property_id
where street_or_unit_no = @street_no 
and property_address = @prop_address
and ss_name = @ss_name

delete p from [dbo].[prospecting_trace_ps_enquiry] p
join [dbo].[prospecting_property] pp
on pp.prospecting_property_id = p.prospecting_property_id
where street_or_unit_no = @street_no 
and property_address = @prop_address
and ss_name = @ss_name

delete c from [dbo].[prospecting_company_property_relationship] c
join [prospecting_property] pp on pp.prospecting_property_id = c.prospecting_property_id
where street_or_unit_no = @street_no 
and property_address = @prop_address
and ss_name = @ss_name

delete from [prospecting_property]
where street_or_unit_no = @street_no 
and property_address = @prop_address
and ss_name = @ss_name
END
