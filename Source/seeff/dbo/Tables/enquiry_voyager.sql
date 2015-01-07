CREATE TABLE [dbo].[enquiry_voyager] (
    [pk_enquiry_voyager_id]          INT           IDENTITY (1, 1) NOT NULL,
    [enquiry_voyager_number]         VARCHAR (50)  NULL,
    [enquiry_voyager_email]          VARCHAR (250) NULL,
    [enquiry_voyager_title]          VARCHAR (50)  NULL,
    [enquiry_voyager_first_name]     VARCHAR (150) NULL,
    [enquiry_voyager_surname]        VARCHAR (150) NULL,
    [enquiry_voyager_date]           VARCHAR (50)  NULL,
    [enquiry_voyager_address_line_1] VARCHAR (250) NULL,
    [enquiry_voyager_address_line_2] VARCHAR (250) NULL,
    [enquiry_voyager_date_added]     DATETIME      CONSTRAINT [DF_enquiry_voyager_enquiry_voyager_date_added_1] DEFAULT (getdate()) NULL,
    [enquiry_voyager_other_info]     TEXT          NULL,
    CONSTRAINT [PK_enquiry_voyager] PRIMARY KEY CLUSTERED ([pk_enquiry_voyager_id] ASC) WITH (FILLFACTOR = 90)
);

