CREATE TABLE [dbo].[webUsers] (
    [webUserId]            NUMERIC (18)   IDENTITY (1, 1) NOT NULL,
    [webUserFirstName]     NVARCHAR (50)  NULL,
    [webUserSurname]       NVARCHAR (50)  NULL,
    [webUserEmail]         NVARCHAR (200) NULL,
    [webUserPassword]      NVARCHAR (100) NULL,
    [webUserDialCode]      NVARCHAR (50)  NULL,
    [webUserWorkTelephone] NVARCHAR (50)  NULL,
    [webUserHomeTelephone] NVARCHAR (50)  NULL,
    [webuserFax]           NVARCHAR (50)  NULL,
    [webUserCell]          NVARCHAR (50)  NULL,
    [fkCountryId]          INT            NULL,
    [fkCurrencyId]         INT            NULL,
    [webUserPropertAlert]  BIT            CONSTRAINT [DF_webUsers_webUserPropertAlert] DEFAULT (0) NOT NULL,
    [webUserOnShow]        BIT            CONSTRAINT [DF_webUsers_webUserOnShow_1] DEFAULT (0) NOT NULL,
    [webUserInsertDate]    SMALLDATETIME  CONSTRAINT [DF_webUsers_webUserInsertDate] DEFAULT (getdate()) NULL,
    [webUserLastLogin]     SMALLDATETIME  NULL,
    [webUserLastAlertSent] DATETIME       NULL,
    CONSTRAINT [PK_webUsers] PRIMARY KEY CLUSTERED ([webUserId] ASC) WITH (FILLFACTOR = 90)
);

