CREATE TABLE [dbo].[propctrl_users] (
    [id]                 INT           IDENTITY (1, 1) NOT NULL,
    [first_name]         VARCHAR (250) NULL,
    [last_name]          VARCHAR (250) NULL,
    [email]              VARCHAR (250) NULL,
    [phone]              VARCHAR (10)  NULL,
    [propctrl_user_id]   INT           NOT NULL,
    [propctrl_user_name] VARCHAR (250) NULL,
    [date_created]       DATETIME      NOT NULL,
    [date_updated]       DATETIME      NOT NULL,
    PRIMARY KEY CLUSTERED ([id] ASC)
);

