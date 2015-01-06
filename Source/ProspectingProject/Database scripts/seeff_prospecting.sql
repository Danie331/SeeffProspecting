use [master]
go
drop database seeff_prospecting
go
create database seeff_prospecting;
go
use seeff_prospecting
go

CREATE TABLE [dbo].[prospecting_company_property_relationship_type](
	[company_property_relationship_type_id] [int] IDENTITY(1,1) NOT NULL,
	[relationship_desc] [varchar](255) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[company_property_relationship_type_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
go

CREATE TABLE [dbo].[prospecting_contact_detail_type](
	[contact_detail_type_id] [int] IDENTITY(1,1) NOT NULL,
	[type_desc] [varchar](255) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[contact_detail_type_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
go

CREATE TABLE [dbo].[prospecting_person_company_relationship_type](
	[person_company_relationship_type_id] [int] IDENTITY(1,1) NOT NULL,
	[relationship_desc] [varchar](255) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[person_company_relationship_type_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
go

CREATE TABLE [dbo].[prospecting_person_person_relationship_type](
	[person_person_relationship_type_id] [int] IDENTITY(1,1) NOT NULL,
	[relationship_desc] [varchar](255) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[person_person_relationship_type_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
go

CREATE TABLE [dbo].[prospecting_person_property_relationship_type](
	[person_property_relationship_type_id] [int] IDENTITY(1,1) NOT NULL,
	[relationship_desc] [varchar](255) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[person_property_relationship_type_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
go

CREATE TABLE [dbo].[prospecting_person_title](
	[prospecting_person_title_id] [int] IDENTITY(1,1) NOT NULL,
	[person_title] [varchar](5) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[prospecting_person_title_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
go

CREATE TABLE [dbo].[prospecting_property](
	[prospecting_property_id] [int] IDENTITY(1,1) NOT NULL,
	[licence_id] [int] NULL,
	[seeff_area_id] [int] NULL,
	[development_id] [int] NULL,
	[lightstone_property_id] [int] NULL,
	[propstats_id] [int] NULL,
	[windeed_id] [int] NULL,
	[erf_no] [int] NULL,
	[portion_no] [int] NULL,
	ss_fh varchar(2) null,
	[property_address] [varchar](255) NULL,
	[street_or_unit_no] [varchar](255) NULL,
	[photo_url] [nvarchar](256) NULL,
	[latitude] [decimal](13, 8) NULL,
	[longitude] [decimal](13, 8) NULL,
	[age] [datetime] NULL,
	[erf_size] [int] NULL,
	[dwell_size] [int] NULL,
	[condition] [varchar](16) NULL,
	[beds] [int] NULL,
	[baths] [int] NULL,
	[receptions] [int] NULL,
	[studies] [int] NULL,
	[garages] [int] NULL,
	[parking_bays] [int] NULL,
	[pool] [bit] NULL,
	[staff_accomodation] [bit] NULL,
	[created_date] [datetime] NULL,
	[updated_date] [datetime] NULL,
	[created_by] [uniqueidentifier] NULL,
	[lightstone_id_or_ck_no] [varchar](255) NULL,
	lightstone_reg_date varchar(8) NULL,
	comments varchar(MAX) null,
	ss_name varchar(255) null,
	ss_number varchar(50) null,
	unit varchar(50) null,
	last_purch_price decimal(18,2) null,
	ss_id varchar(10) null,
	ss_door_number varchar(50) null,
	prospected [bit] NULL
PRIMARY KEY CLUSTERED 
(
	[prospecting_property_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
go

CREATE TABLE [dbo].[prospecting_trace_ps_enquiry](
	[prospecting_trace_ps_enquiry_id] [int] IDENTITY(1,1) NOT NULL,
	[prospecting_property_id] [int] NOT NULL,
	[user] [uniqueidentifier] NOT NULL,
	[id_number] varchar(13) NOT NULL,
	[date_of_enquiry] [datetime] NOT NULL,
	[successful] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[prospecting_trace_ps_enquiry_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[prospecting_trace_ps_enquiry]  WITH CHECK ADD FOREIGN KEY([prospecting_property_id])
REFERENCES [dbo].[prospecting_property] ([prospecting_property_id])
GO


CREATE TABLE [dbo].[prospecting_contact_person](
	[contact_person_id] [int] IDENTITY(1,1) NOT NULL,
	[person_title] [int] NULL,
	[person_gender] varchar(1) not null,
	[id_number] [varchar](14) NOT NULL,
	[firstname] [varchar](255) NOT NULL,
	[surname] [varchar](255) NOT NULL,
	[job_title] [varchar](255) NOT NULL,
	[propcntrl_buyer_id] [int] NULL,
	[referral_network] [bit] NULL,
	[investor] [bit] NULL,

	[is_popi_restricted] bit not null default 0,
	--[popi_first_contact_established] [bit] NOT NULL,
	--[popi_can_store_email] [bit] NULL,
	--[popi_can_store_phone] [bit] NULL,
	[created_date] [datetime] NULL,
	[updated_date] [datetime] NULL,
	[created_by] [uniqueidentifier] NULL,
	comments_notes varchar(max) NULL
PRIMARY KEY CLUSTERED 
(
	[contact_person_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
go
ALTER TABLE [dbo].[prospecting_contact_person]  WITH CHECK ADD FOREIGN KEY([person_title])
REFERENCES [dbo].[prospecting_person_title] ([prospecting_person_title_id])
GO

CREATE TABLE [dbo].[prospecting_person_property_relationship](
	[person_property_relationship_id] [int] IDENTITY(1,1) NOT NULL,
	[contact_person_id] [int] NOT NULL,
	[prospecting_property_id] [int] NOT NULL,
	[relationship_to_property] [int] NOT NULL,
	[created_date] [datetime] NULL,
	[updated_date] [datetime] NULL,
	[created_by] [uniqueidentifier] NULL,
	from_date datetime null,
	to_date datetime null
PRIMARY KEY CLUSTERED 
(
	[person_property_relationship_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[prospecting_person_property_relationship]  WITH CHECK ADD FOREIGN KEY([contact_person_id])
REFERENCES [dbo].[prospecting_contact_person] ([contact_person_id])
GO

ALTER TABLE [dbo].[prospecting_person_property_relationship]  WITH CHECK ADD FOREIGN KEY([prospecting_property_id])
REFERENCES [dbo].[prospecting_property] ([prospecting_property_id])
GO

ALTER TABLE [dbo].[prospecting_person_property_relationship]  WITH CHECK ADD FOREIGN KEY([relationship_to_property])
REFERENCES [dbo].[prospecting_person_property_relationship_type] ([person_property_relationship_type_id])
GO

CREATE TABLE [dbo].[prospecting_person_person_relationship](
	[person_person_relationship_id] [int] IDENTITY(1,1) NOT NULL,
	[contact_person_id] [int] NOT NULL,
	[related_contacted_person_id] [int] NOT NULL,
	[relationship_to_person] [int] NOT NULL,
	[created_date] [datetime] NULL,
	[updated_date] [datetime] NULL,
	[created_by] [uniqueidentifier] NULL,
PRIMARY KEY CLUSTERED 
(
	[person_person_relationship_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[prospecting_person_person_relationship]  WITH CHECK ADD FOREIGN KEY([contact_person_id])
REFERENCES [dbo].[prospecting_contact_person] ([contact_person_id])
GO

ALTER TABLE [dbo].[prospecting_person_person_relationship]  WITH CHECK ADD FOREIGN KEY([related_contacted_person_id])
REFERENCES [dbo].[prospecting_contact_person] ([contact_person_id])
GO

ALTER TABLE [dbo].[prospecting_person_person_relationship]  WITH CHECK ADD FOREIGN KEY([relationship_to_person])
REFERENCES [dbo].[prospecting_person_person_relationship_type] ([person_person_relationship_type_id])
GO

CREATE TABLE [dbo].[prospecting_contact_company](
	[contact_company_id] [int] IDENTITY(1,1) NOT NULL,
	[company_name] [varchar](255) NOT NULL,
	[CK_number] [varchar](255) NULL,
	[created_date] [datetime] NULL,
	[updated_date] [datetime] NULL,
	[created_by] [uniqueidentifier] NULL,
	[company_type] varchar(10) null
PRIMARY KEY CLUSTERED 
(
	[contact_company_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
go

CREATE TABLE [dbo].[prospecting_person_company_relationship](
	[person_company_relationship_id] [int] IDENTITY(1,1) NOT NULL,
	[contact_person_id] [int] NOT NULL,
	[contact_company_id] [int] NULL,
	[relationship_to_company] [int] NULL,
	[created_date] [datetime] NULL,
	[updated_date] [datetime] NULL,
	[created_by] [uniqueidentifier] NULL,
PRIMARY KEY CLUSTERED 
(
	[person_company_relationship_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[prospecting_person_company_relationship]  WITH CHECK ADD FOREIGN KEY([contact_person_id])
REFERENCES [dbo].[prospecting_contact_person] ([contact_person_id])
GO

ALTER TABLE [dbo].[prospecting_person_company_relationship]  WITH CHECK ADD FOREIGN KEY([contact_company_id])
REFERENCES [dbo].[prospecting_contact_company] ([contact_company_id])
GO

ALTER TABLE [dbo].[prospecting_person_company_relationship]  WITH CHECK ADD FOREIGN KEY([relationship_to_company])
REFERENCES [dbo].[prospecting_person_company_relationship_type] ([person_company_relationship_type_id])
GO


CREATE TABLE [dbo].[prospecting_area_dialing_code](
	[prospecting_area_dialing_code_id] int identity(1,1) primary key not null, 
	[dialing_code_id] [int] NOT NULL,
	[code_desc] [varchar](100) NOT NULL
)
GO

CREATE TABLE [dbo].[prospecting_contact_detail](
	[prospecting_contact_detail_id] [int] IDENTITY(1,1) NOT NULL,
	[contact_detail_type] [int] NOT NULL,
	[contact_person_id] [int] NOT NULL,
	[contact_detail] [varchar](255) NOT NULL,
	[intl_dialing_code_id] int foreign key references prospecting_area_dialing_code(prospecting_area_dialing_code_id) null,
	[eleventh_digit] int NULL,
	[is_primary_contact] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[prospecting_contact_detail_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
go

ALTER TABLE [dbo].[prospecting_contact_detail]  WITH CHECK ADD FOREIGN KEY([contact_detail_type])
REFERENCES [dbo].[prospecting_contact_detail_type] ([contact_detail_type_id])
GO

ALTER TABLE [dbo].[prospecting_contact_detail]  WITH CHECK ADD FOREIGN KEY([contact_person_id])
REFERENCES [dbo].[prospecting_contact_person] ([contact_person_id])
GO

CREATE TABLE [dbo].[prospecting_company_property_relationship](
	[company_property_relationship_id] [int] IDENTITY(1,1) NOT NULL,
	[contact_company_id] [int] NOT NULL,
	[prospecting_property_id] [int] NOT NULL,
	[relationship_to_property] [int] NOT NULL,
	[created_date] [datetime] NULL,
	[updated_date] [datetime] NULL,
	[created_by] [uniqueidentifier] NULL,
PRIMARY KEY CLUSTERED 
(
	[company_property_relationship_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[prospecting_company_property_relationship]  WITH CHECK ADD FOREIGN KEY([contact_company_id])
REFERENCES [dbo].[prospecting_contact_company] ([contact_company_id])
GO

ALTER TABLE [dbo].[prospecting_company_property_relationship]  WITH CHECK ADD FOREIGN KEY([prospecting_property_id])
REFERENCES [dbo].[prospecting_property] ([prospecting_property_id])
GO

ALTER TABLE [dbo].[prospecting_company_property_relationship]  WITH CHECK ADD FOREIGN KEY([relationship_to_property])
REFERENCES [dbo].[prospecting_company_property_relationship_type] ([company_property_relationship_type_id])
GO

create table dbo.prospecting_area
(
	prospecting_area_id int not null, -- this is the seeff_area_id (NB!)
	area_name nvarchar(100) not null,
	CONSTRAINT pk_prospecting_area_id primary key (prospecting_area_id)
)
Go

create table dbo.prospecting_kml_area
(
	prospecting_kml_area_id int primary key identity(1,1),
	prospecting_area_id int not null, --this is the seeff_area_id (NB!)
	latitude decimal(18, 10) not null,
	longitude decimal(18, 10) not null,
	area_type char(1) not null,
	seq int not null,
	CONSTRAINT fk_prospecting_area_id FOREIGN KEY (prospecting_area_id) REFERENCES prospecting_area(prospecting_area_id)
)
go

CREATE TABLE [dbo].[prospecting_area_layer](
	[area_layer_id] [int] IDENTITY(1,1) primary key NOT NULL,
	[prospecting_area_id] [int] foreign key references prospecting_area(prospecting_area_id)  NOT NULL,
	[area_type] [varchar](1) NOT NULL,
	[province_id] [int] NULL,
	[formatted_poly_coords] [varchar](max) NOT NULL
)
go


CREATE function [dbo].[point_inside_poly](@point_lat decimal(13,8), @point_lng decimal(13,8), @polygon_coords varchar(max))
returns int
as
begin
	DECLARE @g geography;
	DECLARE @h geography;
	declare @a geometry;
	set @a = geometry::STGeomFromText('POLYGON((' + @polygon_coords + '))', 4236);
	SET @a = @a.MakeValid();

	SET @g = GEOGRAPHY::STGeomFromText(@a.STAsText(),4236);
	IF @g.EnvelopeAngle() >= 90
	BEGIN
		SET	@g = @g.ReorientObject();			
	END;

	SET @h = geography::Point(@point_lng,@point_lat, 4236);
	return @g.STIntersects(@h);
end
go

create procedure [dbo].[find_area_id](@lat decimal(13, 8), @lng decimal(13, 8) , @area_type varchar(1), @province_id int)
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
go

--
create procedure dbo.inside_seeff_area(@lat decimal(13, 8), @lng decimal(13, 8), @seeff_area_id int)
as
begin
	declare @poly_coords varchar(max) = (select formatted_poly_coords from [prospecting_area_layer]
								where prospecting_area_id = @seeff_area_id and area_type = 'R');
	
	declare @result int = 0;
	BEGIN TRY
		SET @result = [dbo].[point_inside_poly](@lat, @lng, @poly_coords);
	END TRY
	BEGIN CATCH
	END CATCH

	return @result;
end;