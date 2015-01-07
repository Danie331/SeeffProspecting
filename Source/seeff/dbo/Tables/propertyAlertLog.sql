CREATE TABLE [dbo].[propertyAlertLog] (
    [propertyAlertLogId]         NUMERIC (18)  IDENTITY (1, 1) NOT NULL,
    [fkpropertyAlertId]          NUMERIC (18)  NULL,
    [propertyAlertLogSmsTo]      NVARCHAR (20) NULL,
    [propertyAlertLogEmailTo]    NVARCHAR (50) NULL,
    [propertyAlertLogInsertDate] DATETIME      NULL,
    CONSTRAINT [PK_propertyAlertLog] PRIMARY KEY CLUSTERED ([propertyAlertLogId] ASC) WITH (FILLFACTOR = 90)
);

