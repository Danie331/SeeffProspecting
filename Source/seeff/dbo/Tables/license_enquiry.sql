CREATE TABLE [dbo].[license_enquiry] (
    [pk_license_enquiry_id]   INT           IDENTITY (1, 1) NOT NULL,
    [license_enquiry_name]    VARCHAR (255) NULL,
    [license_enquiry_surname] VARCHAR (255) NULL,
    [license_enquiry_tel]     VARCHAR (255) NULL,
    [license_enquiry_email]   VARCHAR (255) NULL,
    [license_enquiry_comment] TEXT          NULL,
    [license_enquiry_no]      VARCHAR (255) NULL,
    [license_enquiry_added]   DATETIME      NULL,
    PRIMARY KEY CLUSTERED ([pk_license_enquiry_id] ASC) WITH (FILLFACTOR = 90)
);

