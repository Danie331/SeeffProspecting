CREATE TABLE [dbo].[rental_access] (
    [fkAgentId]    INT           NULL,
    [fkBranchId]   INT           NULL,
    [date]         SMALLDATETIME NULL,
    [branchName]   VARCHAR (50)  NULL,
    [agentName]    VARCHAR (50)  NULL,
    [agentSurname] VARCHAR (50)  NULL
);

