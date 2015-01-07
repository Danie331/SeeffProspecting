CREATE TABLE [dbo].[sms_agent] (
    [pk_sms_agent_id]   INT           IDENTITY (1, 1) NOT NULL,
    [sms_agent_name]    VARCHAR (50)  NULL,
    [sms_agent_email]   VARCHAR (50)  NULL,
    [sms_agent_cell]    VARCHAR (50)  NULL,
    [sms_agent_message] TEXT          NULL,
    [fkAgentId]         INT           NULL,
    [sms_agent_webRef]  VARCHAR (255) NULL,
    [sms_agent_added]   DATETIME      NULL,
    [my_mail_mail_id]   INT           NULL,
    CONSTRAINT [PK_sms_agent] PRIMARY KEY CLUSTERED ([pk_sms_agent_id] ASC) WITH (FILLFACTOR = 90)
);

