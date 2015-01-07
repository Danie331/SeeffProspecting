CREATE TABLE [dbo].[propertyLead_bkp] (
    [propertyLead_bkp_id] INT      IDENTITY (1, 1) NOT NULL,
    [propertyLeadId]      INT      NOT NULL,
    [propertyLeadTypeId]  INT      NULL,
    [fkAgentId]           INT      NULL,
    [fkPropertyId]        INT      NULL,
    [fkBranchId]          INT      NULL,
    [fkAreaId]            INT      NULL,
    [fkProvinceId]        INT      NULL,
    [fkCountryId]         INT      NULL,
    [propertyLeadCount]   INT      NULL,
    [propertyLeadDate]    DATETIME NULL,
    CONSTRAINT [PK_propertyLead_bkp] PRIMARY KEY CLUSTERED ([propertyLead_bkp_id] ASC) WITH (FILLFACTOR = 90)
);

