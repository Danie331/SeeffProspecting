CREATE TABLE [dbo].[agent_enquiry] (
    [pk_agent_enquiry_id]         INT           IDENTITY (1, 1) NOT NULL,
    [agent_enquiry_name]          VARCHAR (255) NULL,
    [agent_enquiry_email]         VARCHAR (255) NULL,
    [agent_enquiry_cell]          VARCHAR (50)  NULL,
    [agent_enquiry_comment]       TEXT          NULL,
    [agent_enquiry_no]            VARCHAR (255) NULL,
    [fkAgentId]                   INT           NULL,
    [fkBranchId]                  INT           NULL,
    [webReference]                VARCHAR (50)  NULL,
    [agent_enquiry_added]         DATETIME      NULL,
    [agent_enquiry_fnb_quicksell] TINYINT       CONSTRAINT [DF_agent_enquiry_agent_enquiry_fnb_quicksell] DEFAULT (0) NULL,
    [agent_subscribe]             TINYINT       DEFAULT (0) NULL,
    [my_mail_mail_id]             INT           NULL,
    CONSTRAINT [PK_agent_enquiry] PRIMARY KEY CLUSTERED ([pk_agent_enquiry_id] ASC) WITH (FILLFACTOR = 90)
);


GO
CREATE NONCLUSTERED INDEX [propertyReference]
    ON [dbo].[agent_enquiry]([webReference] ASC) WITH (FILLFACTOR = 90);


GO
CREATE NONCLUSTERED INDEX [foreignKeys]
    ON [dbo].[agent_enquiry]([fkAgentId] ASC, [fkBranchId] ASC) WITH (FILLFACTOR = 90);

