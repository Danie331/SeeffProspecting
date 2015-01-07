CREATE TABLE [dbo].[sell_enquiry] (
    [pk_sell_enquiry_id]   INT           IDENTITY (1, 1) NOT NULL,
    [sell_enquiry_name]    VARCHAR (255) NULL,
    [sell_enquiry_surname] VARCHAR (255) NULL,
    [sell_enquiry_contact] VARCHAR (255) NULL,
    [sell_enquiry_email]   VARCHAR (255) NULL,
    [sell_enquiry_address] VARCHAR (255) NULL,
    [sell_enquiry_no]      VARCHAR (255) NULL,
    [sell_enquiry_added]   DATETIME      NULL,
    [fkBranchId]           INT           NULL,
    [campaign]             VARCHAR (100) NULL,
    PRIMARY KEY CLUSTERED ([pk_sell_enquiry_id] ASC) WITH (FILLFACTOR = 90)
);

