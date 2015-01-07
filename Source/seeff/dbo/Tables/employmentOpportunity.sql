CREATE TABLE [dbo].[employmentOpportunity] (
    [employmentOpportunityId]          INT             IDENTITY (1, 1) NOT NULL,
    [employmentOpportunityTitle]       NVARCHAR (1000) NULL,
    [employmentOpportunityDescription] NVARCHAR (3000) NULL,
    [fkBranchId]                       INT             NULL,
    [fkAgentId]                        INT             NULL,
    [fkClosingDate]                    SMALLDATETIME   NULL,
    CONSTRAINT [PK_employmentOpportunity] PRIMARY KEY CLUSTERED ([employmentOpportunityId] ASC) WITH (FILLFACTOR = 90)
);

