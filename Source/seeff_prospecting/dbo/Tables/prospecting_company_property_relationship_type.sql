CREATE TABLE [dbo].[prospecting_company_property_relationship_type] (
    [company_property_relationship_type_id] INT           IDENTITY (1, 1) NOT NULL,
    [relationship_desc]                     VARCHAR (255) NOT NULL,
    PRIMARY KEY CLUSTERED ([company_property_relationship_type_id] ASC)
);

