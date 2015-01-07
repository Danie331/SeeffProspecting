CREATE TABLE [dbo].[landlord_enquiry] (
    [pk_landlord_enquiry_id]         INT           IDENTITY (1, 1) NOT NULL,
    [landlord_enquiry_name]          VARCHAR (255) NULL,
    [landlord_enquiry_surname]       VARCHAR (255) NULL,
    [landlord_enquiry_contact]       VARCHAR (255) NULL,
    [landlord_enquiry_address]       VARCHAR (500) NULL,
    [landlord_enquiry_email]         VARCHAR (255) NULL,
    [landlord_enquiry_property_type] VARCHAR (255) NULL,
    [fkBranchId]                     INT           NULL,
    [landlord_enquiry_no]            VARCHAR (255) NULL,
    [landlord_enquiry_added]         DATETIME      NULL,
    PRIMARY KEY CLUSTERED ([pk_landlord_enquiry_id] ASC) WITH (FILLFACTOR = 90)
);

