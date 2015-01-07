CREATE TABLE [dbo].[users] (
    [userId]        INT           IDENTITY (1, 1) NOT NULL,
    [userName]      VARCHAR (50)  NULL,
    [userPassword]  VARCHAR (50)  NULL,
    [userFirstName] VARCHAR (255) NULL,
    [userSurname]   VARCHAR (255) NULL,
    [userEmail]     VARCHAR (200) NULL,
    [userActive]    TINYINT       NULL
);

