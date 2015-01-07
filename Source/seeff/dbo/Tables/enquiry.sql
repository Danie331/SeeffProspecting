CREATE TABLE [dbo].[enquiry] (
    [enquiryId]            NUMERIC (18)    IDENTITY (1, 1) NOT NULL,
    [enquiryFirstname]     NVARCHAR (50)   NULL,
    [enquirySurname]       NVARCHAR (50)   NULL,
    [enquiryDialCode]      CHAR (10)       NULL,
    [enquiryWorkTelephone] NVARCHAR (50)   NULL,
    [enquiryHomeTelephone] NVARCHAR (50)   NULL,
    [enquiryFax]           NVARCHAR (50)   NULL,
    [enquiryCell]          NVARCHAR (50)   NULL,
    [fkCountryId]          INT             CONSTRAINT [DF_enquiry_fkCountryId] DEFAULT (0) NULL,
    [enquiryEmail]         NVARCHAR (50)   NULL,
    [enquiryEmailTo]       NVARCHAR (50)   NULL,
    [enquirySmsTo]         NVARCHAR (20)   NULL,
    [enquiryComments]      NVARCHAR (4000) NULL,
    [enquiryInsertDate]    DATETIME        CONSTRAINT [DF_enquiry_enquiryInsertDate] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_enquiry] PRIMARY KEY CLUSTERED ([enquiryId] ASC) WITH (FILLFACTOR = 90)
);

