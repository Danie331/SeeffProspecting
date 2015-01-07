CREATE TABLE [dbo].[accommodation_enquiry_2010] (
    [pk_accommodation_enquiry_2010_id]               INT            IDENTITY (1, 1) NOT NULL,
    [accommodation_enquiry_2010_type]                VARCHAR (20)   NULL,
    [accommodation_enquiry_2010_reference]           VARCHAR (50)   NULL,
    [accommodation_enquiry_2010_first_name]          NVARCHAR (100) NULL,
    [accommodation_enquiry_2010_surname]             NVARCHAR (100) NULL,
    [accommodation_enquiry_2010_contact_tel]         VARCHAR (50)   NULL,
    [accommodation_enquiry_2010_contact_tel_daytime] VARCHAR (50)   NULL,
    [accommodation_enquiry_2010_email]               VARCHAR (150)  NULL,
    [accommodation_enquiry_2010_city]                VARCHAR (50)   NULL,
    [accommodation_enquiry_2010_city_origin]         VARCHAR (50)   NULL,
    [accommodation_enquiry_2010_rate]                VARCHAR (50)   NULL,
    [accommodation_enquiry_2010_rooms]               VARCHAR (50)   NULL,
    [accommodation_enquiry_2010_persons]             VARCHAR (50)   NULL,
    [accommodation_enquiry_2010_house_apartment]     VARCHAR (50)   NULL,
    [accommodation_enquiry_2010_address]             VARCHAR (255)  NULL,
    [accommodation_enquiry_2010_suburb]              VARCHAR (50)   NULL,
    [accommodation_enquiry_2010_postal_code]         VARCHAR (10)   NULL,
    [accommodation_enquiry_2010_date_available]      VARCHAR (50)   NULL,
    [accommodation_enquiry_2010_date_added]          DATETIME       NULL,
    [accommodation_enquiry_2010_additional_info]     TEXT           NULL,
    CONSTRAINT [PK_accommodation_enquiry_2010] PRIMARY KEY CLUSTERED ([pk_accommodation_enquiry_2010_id] ASC) WITH (FILLFACTOR = 90)
);

