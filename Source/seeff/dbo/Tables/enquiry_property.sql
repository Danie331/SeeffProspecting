CREATE TABLE [dbo].[enquiry_property] (
    [enquiry_propertyId] NUMERIC (18) IDENTITY (1, 1) NOT NULL,
    [fkEnquiryId]        NUMERIC (18) CONSTRAINT [DF_enquiry_property_fkEnquiryId] DEFAULT (0) NOT NULL,
    [fkPropertyId]       NUMERIC (18) CONSTRAINT [DF_enquiry_property_fkPropertyId] DEFAULT (0) NOT NULL,
    CONSTRAINT [PK_enquiry_property] PRIMARY KEY CLUSTERED ([enquiry_propertyId] ASC) WITH (FILLFACTOR = 90)
);

