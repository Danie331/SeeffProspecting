CREATE TABLE [dbo].[agent] (
    [agentId]              INT            IDENTITY (1, 1) NOT NULL,
    [agentFirstName]       NVARCHAR (50)  NULL,
    [agentSurname]         NVARCHAR (50)  NULL,
    [fkAgentTypeId]        INT            NULL,
    [fkAgentRoleId]        INT            NULL,
    [agentEmail]           NVARCHAR (100) CONSTRAINT [DF_agent_agentEmail] DEFAULT (3) NULL,
    [agentLogin]           NVARCHAR (20)  NULL,
    [agentPassword]        CHAR (20)      NULL,
    [fkBranchId]           INT            NULL,
    [agentTelephone]       CHAR (20)      NULL,
    [agentCell]            CHAR (20)      NULL,
    [agentSendSms]         BIT            NULL,
    [agentFax]             CHAR (20)      NULL,
    [agentPhoto]           CHAR (200)     NULL,
    [agentDisplayDetails]  BIT            NULL,
    [agentActive]          BIT            NULL,
    [agentLastLogin]       SMALLDATETIME  NULL,
    [agentAdded]           SMALLDATETIME  CONSTRAINT [DF_agent_agentAdded] DEFAULT (getdate()) NULL,
    [agentBirthday]        SMALLDATETIME  NULL,
    [fkAgentLicenseeId]    INT            CONSTRAINT [DF_agent_fkAgentLicenseeId] DEFAULT (0) NOT NULL,
    [agentPage]            NVARCHAR (200) NULL,
    [partnership_id]       INT            DEFAULT (null) NULL,
    [propCtrl_id]          INT            NULL,
    [viewPropOnly]         INT            CONSTRAINT [DF__agent__viewPropO__38F95D68] DEFAULT (1) NOT NULL,
    [propertyGenieAgentId] VARCHAR (100)  NULL,
    [agentVCard]           VARCHAR (250)  NULL,
    CONSTRAINT [PK_agent] PRIMARY KEY CLUSTERED ([agentId] ASC) WITH (FILLFACTOR = 90)
);


GO
CREATE NONCLUSTERED INDEX [foreignKeys]
    ON [dbo].[agent]([fkAgentTypeId] ASC, [fkAgentRoleId] ASC, [fkBranchId] ASC, [fkAgentLicenseeId] ASC) WITH (FILLFACTOR = 90);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Stores agent details.', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'agent';

