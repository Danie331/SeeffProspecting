CREATE TABLE [dbo].[wl_pg_property_delete] (
    [pg_propertyId]            NUMERIC (18)  IDENTITY (1, 1) NOT NULL,
    [propertyId]               NUMERIC (18)  NOT NULL,
    [propertyReference]        NUMERIC (18)  NOT NULL,
    [propertyGenieSyndicated]  TINYINT       NULL,
    [propertyGenieExpiryDate]  DATETIME      NULL,
    [propertyGenieReference]   NVARCHAR (50) NULL,
    [propertyGenieSuccessDate] DATETIME      NULL,
    [propertyGenieDeleteDate]  DATETIME      NULL,
    [propertyGenieReply]       VARCHAR (400) NULL
);

