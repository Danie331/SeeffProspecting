CREATE TABLE [dbo].[fractionalTitle] (
    [fractionalId]            INT            IDENTITY (1, 1) NOT NULL,
    [fkAreaId]                INT            NULL,
    [fractionalName]          VARCHAR (100)  NULL,
    [fkPropertyTypeId]        INT            NULL,
    [fractionalPrice]         MONEY          NULL,
    [fractionalShortIntro]    VARCHAR (1500) NULL,
    [fractionalImage1]        VARCHAR (100)  NULL,
    [fractionalImage2]        VARCHAR (100)  NULL,
    [fractionalImage3]        VARCHAR (100)  NULL,
    [fractionalImageMain]     VARCHAR (250)  NULL,
    [fractionalImageBanner]   VARCHAR (250)  NULL,
    [fractionalImageLocation] VARCHAR (250)  NULL,
    [fractionalImageFeature]  VARCHAR (250)  NULL,
    [fractionalImageExtra]    VARCHAR (250)  NULL,
    [fractionalImageOverview] VARCHAR (250)  NULL,
    [fractionalActive]        BIT            NULL,
    [fractionalIntro]         TEXT           NULL,
    [fractionalLocation]      TEXT           NULL,
    [fractionalFeatures]      TEXT           NULL,
    [sortOrder]               INT            CONSTRAINT [DF_fractionalTitle_sortOrder] DEFAULT (9999) NULL,
    CONSTRAINT [PK_fractionalTitle] PRIMARY KEY CLUSTERED ([fractionalId] ASC) WITH (FILLFACTOR = 90)
);

