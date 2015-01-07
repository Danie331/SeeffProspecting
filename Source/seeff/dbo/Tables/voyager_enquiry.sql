CREATE TABLE [dbo].[voyager_enquiry] (
    [pk_voyager_enquiry_id]         INT           IDENTITY (1, 1) NOT NULL,
    [voyager_enquiry_name]          VARCHAR (255) NULL,
    [voyager_enquiry_surname]       VARCHAR (255) NULL,
    [voyager_enquiry_tel]           VARCHAR (255) NULL,
    [voyager_enquiry_mobile]        VARCHAR (255) NULL,
    [voyager_enquiry_email]         VARCHAR (255) NULL,
    [voyager_enquiry_buying]        INT           NULL,
    [voyager_enquiry_buying_price]  VARCHAR (255) NULL,
    [voyager_enquiry_selling]       INT           NULL,
    [voyager_enquiry_selling_price] VARCHAR (255) NULL,
    [voyager_enquiry_address]       VARCHAR (255) NULL,
    [voyager_enquiry_suburb]        VARCHAR (255) NULL,
    [voyager_enquiry_city]          VARCHAR (255) NULL,
    [voyager_enquiry_province]      VARCHAR (255) NULL,
    [voyager_enquiry_country]       VARCHAR (255) NULL,
    [voyager_enquiry_added]         DATETIME      NULL,
    [voyager_enquiry_no]            VARCHAR (255) NULL,
    PRIMARY KEY CLUSTERED ([pk_voyager_enquiry_id] ASC) WITH (FILLFACTOR = 90)
);

