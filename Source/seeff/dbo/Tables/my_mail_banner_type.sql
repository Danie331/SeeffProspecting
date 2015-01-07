CREATE TABLE [dbo].[my_mail_banner_type] (
    [type_id] INT          IDENTITY (1, 1) NOT NULL,
    [banner]  VARCHAR (20) NOT NULL,
    PRIMARY KEY CLUSTERED ([type_id] ASC)
);

