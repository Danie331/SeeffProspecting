CREATE TABLE [dbo].[deal_enquiry] (
    [deal_enquiry_id] INT      IDENTITY (1, 1) NOT NULL,
    [registration_id] INT      NOT NULL,
    [deal_id]         INT      NOT NULL,
    [enquiry_date]    DATETIME NOT NULL,
    [no_units]        INT      NOT NULL
);

