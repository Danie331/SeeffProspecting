CREATE TABLE [dbo].[prospecting_contact_detail_type] (
    [contact_detail_type_id] INT           IDENTITY (1, 1) NOT NULL,
    [type_desc]              VARCHAR (255) NOT NULL,
    PRIMARY KEY CLUSTERED ([contact_detail_type_id] ASC)
);

