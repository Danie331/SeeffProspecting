CREATE TABLE [dbo].[propertyLead] (
    [propertyLeadId]     INT      IDENTITY (1, 1) NOT NULL,
    [propertyLeadTypeId] INT      NULL,
    [fkAgentId]          INT      NULL,
    [fkPropertyId]       INT      NULL,
    [fkBranchId]         INT      NULL,
    [fkAreaId]           INT      NULL,
    [fkProvinceId]       INT      NULL,
    [fkCountryId]        INT      NULL,
    [propertyLeadCount]  INT      NULL,
    [propertyLeadDate]   DATETIME NULL,
    CONSTRAINT [PK_propertyLead] PRIMARY KEY CLUSTERED ([propertyLeadId] ASC) WITH (FILLFACTOR = 90)
);

