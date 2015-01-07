CREATE TABLE [dbo].[action] (
    [actionId]        INT           IDENTITY (1, 1) NOT NULL,
    [actionName]      NVARCHAR (50) NULL,
    [actionIsDefault] TINYINT       CONSTRAINT [DF_action_actionIsDefault] DEFAULT (0) NOT NULL,
    CONSTRAINT [PK_action] PRIMARY KEY CLUSTERED ([actionId] ASC) WITH (FILLFACTOR = 90)
);

