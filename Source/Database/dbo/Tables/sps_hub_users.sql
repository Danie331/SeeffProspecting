CREATE TABLE [dbo].[sps_hub_users] (
    [hub_permission_id] INT           IDENTITY (1, 1) NOT NULL,
    [registration_id]   INT           NOT NULL,
    [branch_id]         VARCHAR (100) NOT NULL,
    [user_type]         VARCHAR (50)  NOT NULL,
    [transaction_view]  VARCHAR (100) NULL,
    [email_user_name]   VARCHAR (250) NULL,
    [email_password]    VARCHAR (250) NULL
);

