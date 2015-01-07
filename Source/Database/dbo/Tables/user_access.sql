CREATE TABLE [dbo].[user_access] (
    [registration_id]   BIGINT        NOT NULL,
    [user_hosthame]     VARCHAR (255) NOT NULL,
    [user_appdir]       VARCHAR (MAX) NOT NULL,
    [user_first_access] DATETIME      NOT NULL,
    [user_last_access]  DATETIME      NOT NULL,
    [user_action]       VARCHAR (50)  NOT NULL
);

